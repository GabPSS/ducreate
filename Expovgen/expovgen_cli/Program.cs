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
            Console.WriteLine("Carregando bibliotecas e recursos...");

            //Carregar recursos e definir variáveis
            ImgFetch imgs = new ImgFetch();
            imgs.Logs.TextWritten += Logs_TextWritten;
            string filepath = @"res\text.txt";
            string[] document = File.ReadAllLines(filepath);
            API? langapi = new(document);

            Console.WriteLine("Pronto para iniciar, pressione qualquer tecla...");

            //Etapa 1: Extração de palavras-chave
            Console.Write("Realizando análise de palavras-chave (langapi)...");
            langapi.GetKeywords();
            Console.WriteLine("OK");
            string[]? keywords = langapi.Keywords;

            if (keywords is not null)
            {
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

                //Etapa 2: Busca de imagens para cada palavra-chave
                List<Image> KeywordImages = new List<Image>();

                Random r = new Random();
                DirectoryInfo tempDir = Directory.CreateDirectory(".expovgen_pjtmp");
                string workPath = tempDir.FullName;

                for (int i = 0; i < keywords.Length; i++)
                {
                    string keyword = keywords[i];
                    Console.WriteLine("Buscando imagens para palavra-chave '" + keyword + "' (imgfetch)...");
                    List<Image> images = imgs.RequestImageBitmaps(Services.google, keyword);
                    if (images.Count > 0)
                    {
                        KeywordImages.Add(images[0]);
                        images[0].Save(@"res\imgs\img" + i.ToString("000") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }

                Console.WriteLine(KeywordImages.Count + " imagens baixadas da internet.");
                Console.WriteLine("Pressione qualquer tecla...");
                Console.ReadKey();

                //Etapa 3: Conversão de Texto para voz (audioworks)
                Console.WriteLine("Convertendo texto para voz...");
                RunPY(PyTasks.AudioWorks_TTS, PyEnvs.audworks, @"res\text.txt res\speech.mp3");

                //Etapa 4: Execução do alinhamento forçado com aeneas
                Console.WriteLine("Gerando mapa de sincronização...");
                RunPY(PyTasks.AudioWorks_Align, PyEnvs.audworks, @"res\speech.mp3 res\text.txt res\output.json");

                //Etapa 5: Montagem do vídeo final com moviestitch
            }
            else
            {
                throw new NotImplementedException("Nenhum caminho definido para quando keywords é nulo.");
            }
            
            
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
            Console.WriteLine("OK!");
            Console.ReadKey();
        }
    }
}