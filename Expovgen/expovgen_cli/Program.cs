//using LangAPI;
//using libimgfetch;
using System.Diagnostics;
using System.Drawing;
using Expovgen.ImgFetch;
using Expovgen.LangAPI;

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
            Console.WriteLine(" [5] - Recurso: Junção do vídeo final (indisp.)");
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

            Expovgen vidGen = new();
            vidGen.Logger.TextWritten += Logger_TextWritten;

            switch (input)
            {
                case 0:
                    TodasEtapas(pathTemp, vidGen);
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    goto Start;
                case 1:
                    vidGen.Etapa1(pathTemp);
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    goto Start;
                case 2:
                    Console.Write("Digite uma palavra-chave para a pesquisa: ");
                    string keyword = Console.ReadLine();
                    if (keyword is not null)
                    {
                        vidGen.Etapa2(new string[] { keyword });
                    }
                    else
                    {
                        goto case 2;
                    }
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    goto Start;
                case 3:
                    vidGen.Etapa3();
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    goto Start;
                case 4:
                    vidGen.Etapa4();
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    goto Start;
                case 5:
                    vidGen.Etapa5();
                    Console.WriteLine("Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    goto Start;
            }
        }

        private static void Logger_TextWritten(object source, ExpovgenLogs.TextWrittenEventArgs e)
        {
            Console.WriteLine(e.WrittenText);
        }

        public static void TodasEtapas(string filepath, Expovgen vidGen)
        {            
            Console.WriteLine("Pronto para iniciar, pressione qualquer tecla...");
            Console.ReadKey();

            string[]? keywords = vidGen.Etapa1(filepath);
            if (keywords != null && keywords.Length > 0)
            {
                vidGen.Etapa2(keywords);
                vidGen.Etapa3();
                vidGen.Etapa4();
                vidGen.Etapa5();
            }
            else
            {
                //O que fazer?
            }
        }
    }
}