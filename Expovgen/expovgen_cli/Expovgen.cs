using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangAPI;
using libimgfetch;

namespace Expovgen
{
    public class Expovgen
    {
        public static string[]? Etapa1(string filePath)
        {
            //Etapa 1: Extração de palavras-chave
            string[] document = File.ReadAllLines(filePath);
            API? langapi = new(document);
            Console.WriteLine("--- RECURSO: Extração de palavras-chave ---");
            langapi.GetKeywords();
            Console.WriteLine("Concluído.");
            string[]? keywords = langapi.Keywords;

            //Opcional: Imprimir palavras-chave
            Console.Write("Palavras-chave: ");
            for (int kw = 0; kw < keywords.Length; kw++)
            {
                string keyword = keywords[kw];
                Console.Write("'" + keyword + "'");
                if (kw != keywords.Length - 1)
                {
                    Console.Write(", ");
                }
                else
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();

            return keywords;
        }

        public static void Etapa2(string[] keywords)
        {
            //Etapa 2: Busca de imagens para cada palavra-chave
            Console.WriteLine("--- RECURSO: Pesquisa de imagens ---");

            ImgFetch2 imgs = new ImgFetch2();
            imgs.Logs.TextWritten += Logs_TextWritten;

            imgs.Service = Services.google;
            imgs.RequestQueries = keywords;

            Directory.CreateDirectory(@"res\imgs");

            List<Image> images = imgs.RequestImages().ToList();

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

            Console.WriteLine(images.Count + " imagens baixadas da internet.");
            Console.WriteLine("Pressione qualquer tecla...");
            Console.ReadKey();
        }

        public static void Etapa3()
        {
            //Etapa 3: Conversão de Texto para voz (audioworks)
            Console.WriteLine("Convertendo texto para voz...");
            RunPY(PyTasks.AudioWorks_TTS, PyEnvs.audworks, @"res\text.txt res\speech.mp3");
        }

        public static void Etapa4()
        {
            //Etapa 4: Execução do alinhamento forçado com aeneas
            Console.WriteLine("Gerando mapa de sincronização...");
            RunPY(PyTasks.AudioWorks_Align, PyEnvs.audworks, @"res\speech.mp3 res\text.txt res\output.json");
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

        static void RunPY(PyTasks pytask, PyEnvs pyenv, string args)
        {
            Console.Write("Starting python task " + pytask.ToString() + "...");
            ProcessStartInfo processst = new ProcessStartInfo();
            processst.UseShellExecute = false;
            processst.FileName = PyEnvs_Paths[(int)pyenv];
            processst.Arguments = PyTasks_Paths[(int)pytask] + " " + args;
            //processst.EnvironmentVariables["PATH"] = "audworks\\audioenv\\Scripts\\;audworks\\audioenv\\Lib\\site-packages\\numpy\\core\\include\\numpy;audworks\\audioenv\\Lib\\;audworks\\audioenv\\Lib\\site-packages\\";
            //processst.EnvironmentVariables.Add("PYTHON_HOME", "");
            Process process = Process.Start(processst);
            process.WaitForExit();
            Console.WriteLine("OK! Pressione qualquer tecla...");
            Console.ReadKey();
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
}
