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
    public class Expovgen
    {
        public ExpovgenLogs Logger { get; set; } = new();

        public string[]? Etapa1(string filePath)
        {
            //Etapa 1: Extração de palavras-chave
            string[] document = File.ReadAllLines(filePath);
            //LangAPI1 langapi = new(new string[] { "abc. def?!.ghi", " oi. . tudo bem? " });
            LangAPI1? langapi = new(document);
            langapi.Keywords = new string[] { "linguagem de programação", "principais navegadores", "grande maioria dos sites", "termos Vanilla JavaScript", "principal linguagem", "lado do servidor", "mecanismo JavaScript", "páginas da Web interativas", "alto nível", "linguagem multiparadigma", "navegadores web", "JavaScript", "Vanilla JS", "mecanismos JavaScript", "comunicação assíncrona", "respectivas bibliotecas padrão", "tempo de execução ambientes", "funções de alta ordem", "parte dos navegadores web", "bancos de dados da Web" }; //Override para em caso de cota atingida
            Logger.WriteLine("--- RECURSO 1/5: Extração de palavras-chave ---");
            langapi.GetKeywords();
            Logger.WriteLine("Concluído.");
            string[]? keywords = langapi.Keywords;

            //Opcional: Imprimir palavras-chave
            string toPrint = "Palavras-chave: ";
            for (int kw = 0; kw < keywords.Length; kw++)
            {
                string keyword = keywords[kw];
                toPrint += "'" + keyword + "'";
                if (kw != keywords.Length - 1)
                {
                    toPrint += ", ";
                }
                else
                {
                    Logger.WriteLine(toPrint);
                }
            }

            //Separar frases para alinhamento
            langapi.SplitPhrases();
            langapi.MakeCaptions();
            //Console.WriteLine("Pressione qualquer tecla para continuar...");
            //Console.ReadKey();
            return langapi.Keywords;
            //return keywords;
        }

        public void Etapa2(string[] keywords)
        {
            //Etapa 2: Busca de imagens para cada palavra-chave
            Logger.WriteLine("--- RECURSO 2/5: Pesquisa de imagens ---");

            ImgFetch2 imgs = new(Logger)
            {
                Service = Services.google,
                RequestQueries = keywords
            };

            Directory.CreateDirectory(@"res\imgs");

            List<Image?> images = imgs.RequestImages().ToList();

            if (images.Count > 0)
            {
                for (int x = 0; x < images.Count; x++)
                {
                    try
                    {
                        images[x].Save(@"res\imgs\img" + x.ToString("000") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch { }
                }
            }

            Logger.WriteLine(images.Count + " imagens baixadas da internet.");
            
            //Console.WriteLine("Pressione qualquer tecla...");
            //Console.ReadKey();
        }

        public void Etapa3()
        {
            //Etapa 3: Conversão de Texto para voz (audioworks)
            Logger.WriteLine("--- RECURSO 3/5: Conversão de texto para voz ---");
            Logger.WriteLine("Convertendo texto para voz...(RunPY)");
            RunPY(PyTasks.AudioWorks_TTS, PyEnvs.audworks, @"res\text.txt res\speech.mp3");
        }

        public void Etapa4()
        {
            //Etapa 4: Execução do alinhamento forçado com aeneas
            Logger.WriteLine("--- RECURSO 4/5: Alinhamento Forçado ---");
            Console.WriteLine("Gerando mapa de sincronização...");
            RunPY(PyTasks.AudioWorks_Align, PyEnvs.audworks, @"res\speech.mp3 res\cc.txt res\output.json");
        }

        public void Etapa5()
        {
            //TODO: Implement Etapa 5 (moviestitch)
            Logger.WriteLine("--- RECURSO 5/5: Geração do vídeo final ---");
            throw new NotImplementedException();
        }

        enum PyTasks
        {
            AudioWorks_TTS, AudioWorks_Align
        }

        private static string[] PyTasks_Paths =
        {
            "tts",
            "align"
        };

        enum PyEnvs
        {
            audworks
        }

        private static string[] PyEnvs_Paths =
        {
            @"audworks\Audioworks.exe"
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
            //processst.EnvironmentVariables["PATH"] = "audworks\\audioenv\\Scripts\\;audworks\\audioenv\\Lib\\site-packages\\numpy\\core\\include\\numpy;audworks\\audioenv\\Lib\\;audworks\\audioenv\\Lib\\site-packages\\";
            processst.EnvironmentVariables.Add("IMAGEMAGICK_BINARY", @"C:\Program Files\ImageMagick-7.1.0-Q16-HDRI\magick.exe");
            processst.EnvironmentVariables.Add("PYTHONIOENCODING", "UTF-8");
            Process process = Process.Start(processst);
            process.WaitForExit();
            Logger.WriteLine("Process completed");
            //Console.ReadKey();
        }
    }

    internal class ExpovgenProject
    {
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
