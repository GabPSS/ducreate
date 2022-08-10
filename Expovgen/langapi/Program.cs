using System.IO;

namespace LangAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            string[] document = File.ReadAllLines(filePath);

            API api = new API(document);
            api.GetKeywords();
            
            if (api.Keywords is not null)
            {
                Console.WriteLine("Keywords:");
                foreach (string keyword in api.Keywords)
                {
                    Console.WriteLine(keyword);
                }
            }
            else
            {
                Console.WriteLine("api.Keywords was null");
            }
        }
    }
}