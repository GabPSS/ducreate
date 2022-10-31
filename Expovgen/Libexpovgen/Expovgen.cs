using Expovgen.ImgFetch;
using Expovgen.LangAPI;
using System.Diagnostics;
using System.Drawing;

namespace Expovgen
{
    public enum GenerationType { VideoGen, AudioOnlyGen }
    public enum Etapa1Behaviors { Auto, ForceManual, ForceOneByParagraph, AutoManual }
    public enum Etapa2Behaviors { Auto, ForceManual, AutoManual }
    public enum Etapa3Behaviors { Auto, ForceManual }
    public enum WindowStyle { Simple, Detailed }
    public class Expovgen
    {
        private const string WorkDir = "res";

        #region Public API properties
        public ExpovgenLogs Logger { get; set; } = new();
        public ExpovgenGenerationSettings Settings { get; }

        //Settings for etapas
        #endregion

        #region Public constructor

        public Expovgen(ExpovgenGenerationSettings settings)
        {
            Dispose();
            Settings = settings;
            PyEnvs_Paths[1] = settings.PythonPath == "" ? @"\Python37-32\python.exe" : settings.PythonPath;
        }

        public static void Dispose()
        {
            //Apagar arquivos anteriores

            string[] files = Array.Empty<string>(), folders = Array.Empty<string>();

            try
            {
                files = Directory.EnumerateFiles(WorkDir, "*.*", SearchOption.AllDirectories).ToArray();
            }
            catch { }

            try
            {
                folders = Directory.EnumerateDirectories(WorkDir, "*", SearchOption.AllDirectories).ToArray();
            }
            catch { }

            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
            foreach (string folder in folders)
            {
                try
                {
                    Directory.Delete(folder);
                }
                catch { }
            }
            try
            {
                Directory.Delete(WorkDir);
            }
            catch { }
        }

        #endregion

        #region Etapas 1-5

        #region Etapa 1

        #region Events

        public class Etapa1EventArgs : EventArgs
        {
            public string[] Keywords { get; set; }
            public bool OverrideIntentionallyRequested { get; set; }
        }

        public delegate void Etapa1EventHandler(object source, Etapa1EventArgs e);
        public event Etapa1EventHandler Etapa1Complete;
        public event Etapa1EventHandler Etapa1Failed;

        protected virtual void OnEtapa1Complete(string[] keywords)
        {
            Etapa1Complete?.Invoke(this, new Etapa1EventArgs() { Keywords = keywords });
        }

        protected virtual void OnEtapa1Failed(bool Intentional, string[] keywords)
        {
            Etapa1Failed?.Invoke(this, new Etapa1EventArgs() { Keywords = keywords, OverrideIntentionallyRequested = Intentional });
        }

        #endregion

        public void Etapa1(string filePath = "res\\text.txt")
        {
            Logger.WriteLine("--- RECURSO 1/5: Extração de palavras-chave ---");

            //Cancelar em caso de ForceManual
            if (Settings.Etapa1Behaviors == Etapa1Behaviors.ForceManual)
            {
                OnEtapa1Failed(true, Array.Empty<string>());
                return;
            }
            if (Settings.Etapa1Behaviors == Etapa1Behaviors.ForceOneByParagraph)
            {
                OverrideEtapa1();
                return;
            }

            //Etapa 1: Extração de palavras-chave
            string[] document = File.ReadAllLines(filePath);
            LangAPI1? langapi = new(document);
            try
            {
                langapi.GetKeywords();
            }
            catch (LangAPI1.QuotaExceededException)
            {
                Logger.WriteLine("Recurso de extração de palavras-chave temporariamente indisponível, cota limite da API atingida. Se o problema persistir, tente amanhã");
            }
            catch { }
            Logger.WriteLine("Concluído.");

            if (langapi.Keywords is null || langapi.Keywords?.Length == 0)
            {
                Logger.WriteLine("Erro ao extrair palavras-chave!");
                OnEtapa1Failed(Settings.Etapa1Behaviors == Etapa1Behaviors.AutoManual, Array.Empty<string>());
                return;
            }

            //Separar frases para alinhamento
            langapi.SplitPhrases();
            langapi.MakeCaptions();

            //Retornar adequadamente
            if (Settings.Etapa1Behaviors == Etapa1Behaviors.AutoManual)
            {
                OnEtapa1Failed(true, langapi.Keywords);
            }

            OnEtapa1Complete(langapi.Keywords);
        }

        /// <summary>
        /// Prepare all text files for the next steps as <seealso cref="Etapa1(string)"/>, but treat keywords not as words but entire paragraphs (ideal for presentation modes)
        /// </summary>
        public void OverrideEtapa1()
        {
            File.Copy("res\\text.txt", "res\\split.txt");
            File.Copy("res\\text.txt", "res\\keywords.txt");
            string[] outputLines = File.ReadAllLines("res\\text.txt");
            LangAPI1 langAPI = new(outputLines);
            langAPI.MakeCaptions();
            Logger.WriteLine("Override completo");
            OnEtapa1Complete(outputLines);
        }

        /// <summary>
        /// Prepare all text files for the next steps as <seealso cref="Etapa1(string)"/>, but use <paramref name="inputKeywords"/> instead of requesting the keywords through the web API.
        /// </summary>
        /// <param name="inputKeywords"></param>
        /// <param name="filePath"></param>
        public void OverrideEtapa1(string[] inputKeywords, string filePath = "res\\text.txt")
        {
            try
            {
                string[] doc = File.ReadAllLines(filePath);
                File.WriteAllLines("res\\keywords.txt", inputKeywords);
                LangAPI1 langAPI = new(doc) { Keywords = inputKeywords };
                langAPI.SplitPhrases();
                langAPI.MakeCaptions();
                Logger.WriteLine("Override completo");
                OnEtapa1Complete(langAPI.Keywords);
            }
            catch (UserForceCancelException) { }
        }

        #endregion

        #region Etapa 2

        public class Etapa2EventArgs : EventArgs
        {
            public List<Image?> Images { get; set; }
            public string[] RequestQueries { get; set; }
            public bool OverrideRequestedIntentionally { get; set; } = false;
        }

        public delegate void Etapa2EventHandler(object sender, Etapa2EventArgs e);
        public event Etapa2EventHandler Etapa2Complete;
        public event Etapa2EventHandler Etapa2Incomplete;

        /// <summary>
        /// Standard method called when Etapa2 returns work
        /// </summary>
        /// <param name="images">The images generated (if not, send in an empty array)</param>
        /// <param name="requestQueries">The queries requested</param>
        /// <param name="TotallySuccessful">True if the step was completely successful and no unrequested user intervention is required. Otherwise, false.</param>
        /// <param name="overrideIntentional">True if TotallySuccessful=false was intentional, due to the user requesting intervention, otherwise false</param>
        protected virtual void OnEtapa2Done(List<Image?> images, string[] requestQueries, bool TotallySuccessful = true, bool overrideIntentional = false)
        {
            if (TotallySuccessful)
            {
                Etapa2Complete?.Invoke(this, new Etapa2EventArgs() { Images = images, RequestQueries = requestQueries });
            }
            else
            {
                Etapa2Incomplete?.Invoke(this, new Etapa2EventArgs() { Images = images, RequestQueries = requestQueries, OverrideRequestedIntentionally = overrideIntentional });
            }
        }

        public void Etapa2(string[] keywords)
        {
            Logger.WriteLine("--- RECURSO 2/5: Pesquisa de imagens ---");
            try
            {
                if (Settings.Etapa1Behaviors == Etapa1Behaviors.ForceOneByParagraph || Settings.Etapa2Behaviors == Etapa2Behaviors.ForceManual)
                {
                    OnEtapa2Done(new Image?[keywords.Length].ToList<Image?>(), keywords, false, true);
                    return;
                }

                //Etapa 2: Busca de imagens para cada palavra-chave

                ImgFetch2 fetcher = new(Logger, (Settings.VideoWidth,Settings.VideoHeight))
                {
                    Service = Settings.ImgFetchService,
                    RequestQueries = keywords,
                    ServicePreferences = null,
                    OverlayKeywordOnImage = Settings.ShowKeywordOnImage
                };

                try
                {
                    List<Image?> images = fetcher.RequestImages().ToList();
                    bool TotallySuccessful = SaveImgfetchPictures(images);

                    Logger.WriteLine(images.Count + " imagens baixadas da internet.");

                    if (Settings.Etapa2Behaviors == Etapa2Behaviors.AutoManual)
                    {
                        OnEtapa2Done(images, keywords, false, true);
                        return;
                    }
                    OnEtapa2Done(images, fetcher.RequestQueries, TotallySuccessful, false);
                    return;
                }
                catch
                {
                    OnEtapa2Done(new Image?[keywords.Length].ToList<Image?>(), keywords, false, false);
                    return;
                }

            }
            catch (UserForceCancelException) { }
        }

        public static bool SaveImgfetchPictures(List<Image?> images)
        {
            Directory.CreateDirectory(@"res\imgs");
            bool TotallySuccessful = true; ;
            if (images.Count > 0)
            {
                for (int x = 0; x < images.Count; x++)
                {                     
                    try
                    {
                        images[x].Save(@"res\imgs\" + x.ToString("000") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                        TotallySuccessful = false;
                    }  
                }
            }
            return TotallySuccessful;
        }

        #endregion

        #region Etapa 3

        #region Etapa 3 Events

        public class Etapa3EventArgs : EventArgs
        {
            public bool OverrideIntentional { get; set; }
        }

        public delegate void Etapa3EventHandler(object sender, Etapa3EventArgs e);
        public event Etapa3EventHandler Etapa3Completed;
        public event Etapa3EventHandler Etapa3Failed;

        protected virtual void OnEtapa3Completed()
        {
            Etapa3Completed?.Invoke(this, new Etapa3EventArgs() { OverrideIntentional = false });
        }

        protected virtual void OnEtapa3Failed(bool Intentional = false)
        {
            Etapa3Failed?.Invoke(this, new Etapa3EventArgs() { OverrideIntentional = Intentional });
        }

        #endregion

        public void Etapa3()
        {
            try
            {
                if (Settings.Etapa3Behaviors == Etapa3Behaviors.ForceManual)
                {
                    OnEtapa3Failed(true);
                    return;
                }

                //Etapa 3: Conversão de Texto para voz (audioworks)
                Logger.WriteLine("--- RECURSO 3/5: Conversão de texto para voz ---");
                Logger.WriteLine("Convertendo texto para voz...(RunPY)");
                int result = RunPY(PyTasks.AudioWorks_TTS, PyEnvs.audworks, @"res\text.txt res\speech.mp3");
                if (result == 0)
                    OnEtapa3Completed();
                else
                    OnEtapa3Failed(false);
            }
            catch (UserForceCancelException) { }
        }

        #endregion

        #region Etapa 4

        #region Etapa 4 Events

        public delegate void Etapa4EventHandler(object sender, EventArgs e);
        public event Etapa4EventHandler Etapa4Completed;
        public event Etapa4EventHandler Etapa4Failed;

        protected virtual void OnEtapa4Completed()
        {
            Etapa4Completed?.Invoke(this, new EventArgs());
        }

        protected virtual void OnEtapa4Failed()
        {
            Etapa4Failed?.Invoke(this, new EventArgs());
        }

        #endregion

        public void Etapa4()
        {
            try
            {
                //Etapa 4: Execução do alinhamento forçado com aeneas
                Logger.WriteLine("--- RECURSO 4/5: Alinhamento Forçado ---");
                Console.WriteLine("Gerando mapa de sincronização...");
                //RunPY(PyTasks.AudioWorks_Align, PyEnvs.audworks, @"res\speech.mp3 res\cc.txt res\output.json");
                int r1 = RunPY(PyTasks.Aeneas_cmdline, PyEnvs.python_inst, "res\\speech.mp3 res\\cc.txt \"task_language=por|is_text_type=plain|os_task_file_format=txt\" res\\ccmap.txt");
                if (r1 != 0)
                {
                    OnEtapa4Failed();
                    return;
                }
                int r2 = RunPY(PyTasks.Aeneas_cmdline, PyEnvs.python_inst, "res\\speech.mp3 res\\split.txt \"task_language=por|is_text_type=plain|os_task_file_format=txt\" res\\splitmap.txt");
                if (r2 != 0)
                {
                    OnEtapa4Failed();
                    return;
                }
                LangAPI1.PrepareAlignmentAgainstKeywords("res\\keywords.txt", "res\\splitmap.txt");
                OnEtapa4Completed();
            }
            catch (UserForceCancelException) { }
        }

        #endregion

        #region Etapa 5

        #region Etapa 5 Events

        public delegate void Etapa5EventHandler(object sender, EventArgs e);
        public event Etapa5EventHandler Etapa5Completed;
        public event Etapa5EventHandler Etapa5Failed;

        protected virtual void OnEtapa5Completed()
        {
            Etapa5Completed?.Invoke(this, new EventArgs());
        }

        protected virtual void OnEtapa5Failed()
        {
            Etapa5Failed?.Invoke(this, new EventArgs());
        }

        #endregion
        public void Etapa5()
        {
            try
            {
                Logger.WriteLine("--- RECURSO 5/5: Geração do vídeo final ---");
                bool AddMusic = false;
                //Prepare Etapa 5
                if (Settings.BackgroundMusicPath is not null)
                {
                    try
                    {
                        File.Copy(Settings.BackgroundMusicPath, "res\\music.mp3");
                        AddMusic = true;
                    }
                    catch
                    {
                        Logger.WriteLine("(!) Erro ao acessar a música de fundo, vídeo resultante não a conterá");
                    }
                }

                // Prepare title card and credits card if present
                if (Settings.Credits == null)
                {
                    Settings.Credits = Array.Empty<string>();
                }
                File.WriteAllLines("res\\card_credits.txt", Settings.Credits);
                if (Settings.TitleCard == null)
                {
                    Settings.TitleCard = Array.Empty<string>();
                }
                File.WriteAllLines("res\\card_title.txt", Settings.TitleCard);



                int r = RunPY(PyTasks.Moviepy_Script, PyEnvs.python_inst, Settings.VideoWidth + " " + Settings.VideoHeight + " " + AddMusic.ToString() + " " + Settings.FontSize + " " + (Settings.BackgroundVolume < 0.99 ? "0." + Math.Round(Settings.BackgroundVolume * 100) : 1));
                if (r == 0)
                {
                    File.Copy("res\\output.mp4", Settings.ExportPath);
                    OnEtapa5Completed();
                }
                else
                {
                    OnEtapa5Failed();
                }
            }
            catch (UserForceCancelException) { }
        }

        #endregion

        #endregion

        #region Python Interop

        private enum PyTasks
        {
            AudioWorks_TTS, AudioWorks_Align, Aeneas_cmdline, Moviepy_Script
        }

        private static readonly string[] PyTasks_Paths =
        {
            "tts",
            "align",
            "-m aeneas.tools.execute_task",
            "moviestitch.py"
        };

        private enum PyEnvs
        {
            audworks,
            python_inst
        }

        private readonly string[] PyEnvs_Paths =
        {
            @"audworks\Audioworks.exe",
            "(python.exe path, see constructor)",
        };

        /// <summary>
        /// Starts a python instance to run a preset python task
        /// </summary>
        /// <param name="pytask">The task string to run</param>
        /// <param name="pyenv">The environment/compiled script on which to run the task</param>
        /// <param name="args">String arguments to pass</param>
        /// <returns>an <see cref="int"/> corresponding to the process's exit code</returns>
        private int RunPY(PyTasks pytask, PyEnvs pyenv, string args)
        {
            Logger.WriteLine("Starting python task " + pytask.ToString() + "...");
            ProcessStartInfo processst = new()
            {
                UseShellExecute = false,
                FileName = PyEnvs_Paths[(int)pyenv],
                Arguments = PyTasks_Paths[(int)pytask] + " " + args,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            if (pyenv == PyEnvs.python_inst && Settings.PythonPath == "") //TODO: Test this out
            {
                processst.FileName = processst.EnvironmentVariables["SystemDrive"] + processst.FileName;
            }
            processst.EnvironmentVariables.Add("IMAGEMAGICK_BINARY", @"C:\Program Files\ImageMagick-7.1.0-Q16-HDRI\magick.exe");
            processst.EnvironmentVariables.Add("PYTHONIOENCODING", "UTF-8");

            Process? process = Process.Start(processst);
            if (process != null)
            {
                process.OutputDataReceived += WritePythonDataToLog;
                process.ErrorDataReceived += WritePythonDataToLog;
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();
                process.CancelErrorRead();
                process.CancelOutputRead();
                if (process.ExitCode != 0)
                {
                    Logger.WriteLine("Python step " + pytask.ToString() + " failed");
                }
                else
                {
                    Logger.WriteLine("Process completed");
                }
                return process.ExitCode;
            }
            else
            {
                return -1;
            }
        }

        private void WritePythonDataToLog(object sender, DataReceivedEventArgs e)
        {
            Logger.WriteLine("[RunPY] " + e.Data);
        }

        #endregion
    }

    /// <summary>
    /// An exception thrown when the user cancels the generation process
    /// </summary>
    public class UserForceCancelException : Exception
    {

    }

    /// <summary>
    /// A class that handles logging in Expovgen
    /// </summary>
    public class ExpovgenLogs
    {
        public List<string> Log = new();


        public class TextWrittenEventArgs : EventArgs
        {
            public string WrittenText { get; set; } = "";
        }

        public delegate void TextWrittenEventHandler(object source, TextWrittenEventArgs e);
        public event TextWrittenEventHandler? TextWritten;

        protected virtual void OnTextWritten(string text)
        {
            TextWritten?.Invoke(this, new TextWrittenEventArgs() { WrittenText = text });
        }

        public void WriteLine(string text)
        {
            Log.Add(text);
            OnTextWritten(text);
        }
    }
}
