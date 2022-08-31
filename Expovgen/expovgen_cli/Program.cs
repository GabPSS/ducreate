using LangAPI;
using libimgfetch;
using System.Diagnostics;
using System.Drawing;

namespace Expovgen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Start:
            Console.Clear();
            Console.WriteLine("Bem-vindo(a) ao expovgen!");
            Console.WriteLine("=========================");
            Console.WriteLine();
            Console.WriteLine("  AVISO: No estado atual, o EXPOVGEN não está em estado 'feature-complete', portanto alguns recursos não estarão disponíveis por não terem sido implementados até então.");
            Console.WriteLine("  IMPORTANTE: Para funcionar, o programa requere que um arquvo de texto 'text.txt' esteja situado em uma pasta res\\ acessível neste ambiente!");
            Console.WriteLine();
            Console.WriteLine("Opções disponíveis:");
            Console.WriteLine("");
            Console.WriteLine(" [0] - Teste de Algoritmo final");
            Console.WriteLine(" [1] - Recurso: Extração de palavras-chave");
            Console.WriteLine(" [2] - Recurso: Pesquisa de imagens");
            Console.WriteLine(" [3] - Recurso: Conversão de texto para voz");
            Console.WriteLine(" [4] - Recurso: Alinhamento forçado");
            Console.WriteLine("     - Recurso: Junção do vídeo final (indisp.)");
        input:
            Console.WriteLine("");
            Console.Write("Digite a opção desejada: ");
            int input;
            try
            {
                input = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                goto input;
            }
            
            string pathTemp = @"res\text.txt";

            switch (input)
            {
                case 0:
                    TodasEtapas(pathTemp);
                    goto Start;
                case 1:
                    Etapa1(pathTemp);
                    goto Start;
                case 2:
                    Console.Write("Digite uma palavra-chave para a pesquisa: ");
                    string keyword = Console.ReadLine();
                    if (keyword is not null)
                    {
                        Etapa2(new string[] { keyword });
                    }
                    else
                    {
                        goto case 2;
                    }
                    goto Start;
                case 3:
                    Etapa3();
                    goto Start;
                case 4:
                    Etapa4();
                    goto Start;
            }
        }

        public static void TodasEtapas(string filepath)
        {            
            Console.WriteLine("Pronto para iniciar, pressione qualquer tecla...");
            Console.ReadKey();

            string[]? keywords = Etapa1(filepath);
            if (keywords != null && keywords.Length > 0)
            {
                Etapa2(keywords);
                Etapa3();
                Etapa4();
            }
            else
            {
                //O que fazer?
            }
        }

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
            List<Image> KeywordImages = new List<Image>();
            ImgFetch imgs = new ImgFetch();
            //imgs.MaxDownloads = 1;
            imgs.Logs.TextWritten += Logs_TextWritten;
            Random r = new Random();
            Directory.CreateDirectory(@"res\imgs");
            //string workPath = tempDir.FullName;

            for (int i = 0; i < keywords.Length; i++)
            {
                string keyword = keywords[i];
                Console.WriteLine("Buscando imagens para palavra-chave '" + keyword + "' (imgfetch)...");
                List<Image> images = imgs.RequestImageBitmaps(new ImgFetchRequest()
                {
                    RequestingService = Services.google,
                    SearchQueries = new List<string>() { "Apple", "Banana", "Orange" },
                    EnablePoolDownloads = true
                });
                    //imgs.RequestImageBitmaps(Services.google, keyword);
                if (images.Count > 0)
                {
                    KeywordImages.AddRange(images);
                    for (int x = 0; x < images.Count; x++)
                    {
                        images[x].Save(@"res\imgs\img" + x.ToString("000") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }

            Console.WriteLine(KeywordImages.Count + " imagens baixadas da internet.");
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

        private static void Logs_TextWritten(object source, ImgFetchLogs.TextWrittenEventArgs e)
        {
            Console.WriteLine("(imgfetch) " + e.WrittenText);
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
}