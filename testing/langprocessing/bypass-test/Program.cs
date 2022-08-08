using System.IO;
using libimgfetch;

namespace Bypass
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] text = File.ReadAllLines("article.txt");
            List<string> wordList = new List<string>();
            for (int x = 0; x < text.Length; x++)
            {
                //for each line
                string line = text[x];
                line = line.ToLower();
                string[] words = line.Split(new List<char>(){' ',
                '.',
                ',',
                '?',
                '!',
                ';',
                ':',
                '(',
                ')',
                '-',
                '/',
                '\\',
                '"',
                '%'}.ToArray());   
                foreach (string word in words)
                {
                    if (!wordList.Contains(word))
                    {
                        wordList.Add(word);
                    }
                }
            }
            Console.WriteLine("Words:");
            int i = 0;
            foreach (string word in wordList)
            {
                Console.WriteLine(word);
                ImgFetch fetcher = new ImgFetch();
                Stream? files = fetcher.RequestSingleImage(new ImgFetchRequest() {RequestingService = Services.google, SearchQuery = word, ServicePreferences = new GooglePreferences() {UseCCLicense = true}});
                
                if (files is not null)
                {
                    FileStream fStream = File.OpenWrite(i.ToString("000"));
                    files.CopyTo(fStream);
                    fStream.Flush();
                    fStream.Close();
                }
                i++;
            }
            Console.WriteLine("--- END ---");
        }
    }
}