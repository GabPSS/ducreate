using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Expovgen.ImgFetch;
using Expovgen.LangAPI;

namespace Expovgen
{

    public enum Etapa1Behaviors { Auto, ForceManual, ForceOneByParagraph, AutoManual }
    public enum Etapa2Behaviors { Auto, ForceManual, AutoManual }
    public enum Etapa3Behaviors { Auto, ForceManual }
    public enum Etapa4Behaviors { Auto, None }

    public class Expovgen
    {
        private const string WorkDir = "res";

        #region Public API properties
        public ExpovgenLogs Logger { get; set; } = new();
        public (int Width, int Height) VideoDimensions { get; set; } = (1366, 768);

        //Settings for etapas
        public Etapa1Behaviors Etapa1Behavior { get; set; } = Etapa1Behaviors.Auto;
        public Etapa2Behaviors Etapa2Behavior { get; set; } = Etapa2Behaviors.Auto;
        public Etapa3Behaviors Etapa3Behavior { get; set; } = Etapa3Behaviors.Auto;
        public Etapa4Behaviors Etapa4Behavior { get; set; } = Etapa4Behaviors.Auto;

        #endregion

        public Expovgen()
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
            Etapa1Complete(this, new Etapa1EventArgs() { Keywords = keywords });
        }

        protected virtual void OnEtapa1Failed(bool Intentional, string[] keywords)
        {
            Etapa1Failed(this, new Etapa1EventArgs() { Keywords = keywords, OverrideIntentionallyRequested = Intentional });
        }

        #endregion

        public void Etapa1(string filePath = "res\\text.txt")
        {
            //Cancelar em caso de ForceManual
            if (Etapa1Behavior == Etapa1Behaviors.ForceManual)
            {
                OnEtapa1Failed(true, Array.Empty<string>());
                return;
            }
            if (Etapa1Behavior == Etapa1Behaviors.ForceOneByParagraph)
            {
                OverrideEtapa1();
                return;
            }

            //Etapa 1: Extração de palavras-chave
            string[] document = File.ReadAllLines(filePath);
            LangAPI1? langapi = new(document);
            Logger.WriteLine("--- RECURSO 1/5: Extração de palavras-chave ---");
            langapi.GetKeywords(); 
            Logger.WriteLine("Concluído.");

            if (langapi.Keywords is null)
            {
                Logger.WriteLine("Erro ao extrair palavras-chave!");
                OnEtapa1Failed(false, Array.Empty<string>());
                return;
            }

            //Separar frases para alinhamento
            langapi.SplitPhrases();
            langapi.MakeCaptions();

            //Retornar adequadamente
            if (Etapa1Behavior == Etapa1Behaviors.AutoManual)
            {
                OnEtapa1Failed(true,langapi.Keywords);
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
            LangAPI1 langAPI = new LangAPI1(outputLines);
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
            string[] doc = File.ReadAllLines(filePath);
            LangAPI1 langAPI = new LangAPI1(doc);
            langAPI.Keywords = inputKeywords;
            langAPI.SplitPhrases();
            langAPI.MakeCaptions();
            Logger.WriteLine("Override completo");
            OnEtapa1Complete(langAPI.Keywords);
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
                Etapa2Complete(this, new Etapa2EventArgs() { Images = images, RequestQueries = requestQueries });
            }
            else
            {
                Etapa2Incomplete(this, new Etapa2EventArgs() { Images = images, RequestQueries = requestQueries, OverrideRequestedIntentionally = overrideIntentional });
            }
        }

        public void Etapa2(string[] keywords)
        {
            if (Etapa1Behavior == Etapa1Behaviors.ForceOneByParagraph || Etapa2Behavior == Etapa2Behaviors.ForceManual)
            {
                OnEtapa2Done(new Image?[keywords.Length].ToList<Image?>(), keywords, false, true);
                return;
            }

            //Etapa 2: Busca de imagens para cada palavra-chave
            Logger.WriteLine("--- RECURSO 2/5: Pesquisa de imagens ---");

            ImgFetch2 fetcher = new(Logger, VideoDimensions)
            {
                Service = Services.google,
                RequestQueries = keywords
            };


            List<Image?> images = fetcher.RequestImages().ToList();
            bool TotallySuccessful = SaveImgfetchPictures(images);
            
            Logger.WriteLine(images.Count + " imagens baixadas da internet.");
            
            if (Etapa2Behavior == Etapa2Behaviors.AutoManual)
            {
                OnEtapa2Done(images, keywords, false, true);
            }

            OnEtapa2Done(images, fetcher.RequestQueries, TotallySuccessful,false);
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
            Etapa3Completed(this, new Etapa3EventArgs() { OverrideIntentional = false });
        }

        protected virtual void OnEtapa3Failed(bool Intentional = false)
        {
            Etapa3Failed(this, new Etapa3EventArgs() { OverrideIntentional = Intentional });
        }

        #endregion


        public void Etapa3()
        {
            if (Etapa3Behavior == Etapa3Behaviors.ForceManual)
            {
                OnEtapa3Failed(true);
                return;
            }

            //Etapa 3: Conversão de Texto para voz (audioworks)
            Logger.WriteLine("--- RECURSO 3/5: Conversão de texto para voz ---");
            Logger.WriteLine("Convertendo texto para voz...(RunPY)");
            RunPY(PyTasks.AudioWorks_TTS, PyEnvs.audworks, @"res\text.txt res\speech.mp3");
            OnEtapa3Completed();
        }

        #endregion

        public void Etapa4()
        {
            //Etapa 4: Execução do alinhamento forçado com aeneas
            Logger.WriteLine("--- RECURSO 4/5: Alinhamento Forçado ---");
            Console.WriteLine("Gerando mapa de sincronização...");
            //RunPY(PyTasks.AudioWorks_Align, PyEnvs.audworks, @"res\speech.mp3 res\cc.txt res\output.json");
            RunPY(PyTasks.Aeneas_cmdline, PyEnvs.python_inst, "res\\speech.mp3 res\\cc.txt \"task_language=por|is_text_type=plain|os_task_file_format=txt\" res\\ccmap.txt");
            RunPY(PyTasks.Aeneas_cmdline, PyEnvs.python_inst, "res\\speech.mp3 res\\split.txt \"task_language=por|is_text_type=plain|os_task_file_format=txt\" res\\splitmap.txt");
            LangAPI1.PrepareAlignmentAgainstKeywords("res\\keywords.txt", "res\\splitmap.txt");
        }

        public void Etapa5()
        {
            //TODO: Implement Etapa 5 (moviestitch)
            Logger.WriteLine("--- RECURSO 5/5: Geração do vídeo final ---");
            RunPY(PyTasks.Moviepy_Script, PyEnvs.python_inst, "");
        }

        #region Python Interop

        enum PyTasks
        {
            AudioWorks_TTS, AudioWorks_Align, Aeneas_cmdline, Moviepy_Script
        }

        private static string[] PyTasks_Paths =
        {
            "tts",
            "align",
            "-m aeneas.tools.execute_task",
            "stitchtest2.py"
        };

        enum PyEnvs
        {
            audworks,
            python_inst
        }

        private static string[] PyEnvs_Paths =
        {
            @"audworks\Audioworks.exe",
            @"\Python37-32\python.exe", //TODO: Replace this path with env variables
        };

        void RunPY(PyTasks pytask, PyEnvs pyenv, string args)
        {
            Logger.WriteLine("Starting python task " + pytask.ToString() + "...");
            ProcessStartInfo processst = new()
            {
                UseShellExecute = false,
                FileName = PyEnvs_Paths[(int)pyenv],
                Arguments = PyTasks_Paths[(int)pytask] + " " + args,
            };
            if (pyenv == PyEnvs.python_inst)
            {
                processst.FileName = processst.EnvironmentVariables["SystemDrive"] + processst.FileName;
            }
            //processst.EnvironmentVariables["PATH"] = "audworks\\audioenv\\Scripts\\;audworks\\audioenv\\Lib\\site-packages\\numpy\\core\\include\\numpy;audworks\\audioenv\\Lib\\;audworks\\audioenv\\Lib\\site-packages\\";
            processst.EnvironmentVariables.Add("IMAGEMAGICK_BINARY", @"C:\Program Files\ImageMagick-7.1.0-Q16-HDRI\magick.exe");
            processst.EnvironmentVariables.Add("PYTHONIOENCODING", "UTF-8");
            Process process = Process.Start(processst);
            process.WaitForExit();
            Logger.WriteLine("Process completed");
            //Console.ReadKey();
        }

        #endregion
    }

    internal class ExpovgenProject
    {
        //TODO: Make it save previous keywords used
        public string Title { get; set; }
        public string Description { get; set; }
        public string? FilePath { get; set; }
        public string[] Document;

        public ExpovgenProject()
        {
            Document = Array.Empty<string>();
        }

        public ExpovgenProject(string filePath)
        {
            //TODO: Load attributes from an unidentified file type
            throw new NotImplementedException();
        }
    }

    public class ExpovgenLogs
    {
        public List<string> Log = new List<string>();
        

        public class TextWrittenEventArgs : EventArgs
        {
            public string WrittenText { get; set; } = "";
        }

        public delegate void TextWrittenEventHandler(object source, TextWrittenEventArgs e);
        public event TextWrittenEventHandler TextWritten;

        protected virtual void OnTextWritten(string text)
        {
            if (TextWritten != null)
            {
                TextWritten(this, new TextWrittenEventArgs() { WrittenText = text });
            }
        }

        public void WriteLine(string text)
        {
            Log.Add(text);
            OnTextWritten(text);
        }
    }
}
