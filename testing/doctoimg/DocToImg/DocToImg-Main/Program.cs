using System.Drawing;
using LangAPI;
using libimgfetch;

namespace DocToImg
{
    internal class Program
    {
        static void Main(string[] args)
        {
            main:
            Console.Write("Please enter the file's path: ");
            string filePath = Console.ReadLine();
            if (!File.Exists(filePath))
            {
                goto main;
            }

            string[] file = File.ReadAllLines(filePath);
            Console.WriteLine("File contents: ---------------");
            foreach (string line in file)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("\nPress any key to start the operation...");
            Console.ReadKey();
            API langApi = new API(file);
            langApi.GetKeywords();
            if (langApi.Keywords is not null)
            {
                Console.WriteLine("Keywords obtained: -------------");
                foreach (string keyword in langApi.Keywords)
                {
                    Console.WriteLine(keyword);
                }
                Console.WriteLine("\nPress any key to confirm search operations...");
                Console.ReadKey();
                List<Image> images = new List<Image>();

                ImgFetch imgFetch = new ImgFetch();
                foreach (string keyword in langApi.Keywords)
                {
                    List<Image> imageList = imgFetch.RequestImageBitmaps(Services.google, keyword);
                    images.AddRange(imageList);
                }

                for (int i = 0; i < images.Count; i++)
                {
                    images[i].Save(i.ToString("000"));
                }
                Console.WriteLine("Images saved!");
            }
        }
    }
}