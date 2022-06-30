using System.IO;
using System.Collections.Generic;
using libimgfetch;

namespace imgfimptest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Stream> Files = ImgFetch.GetImageStreams(Services.google,"dog");
            for (int i = 0;i<Files.Count;i++)
            {
                try
                {
                    FileStream filewrite = File.OpenWrite(i.ToString("000"));
                    Files[i].CopyTo(filewrite);
                    filewrite.Close();
                }
                catch (Exception x)
                {
                    Console.Error.WriteLine("(Writing error) " + x.Message);
                }
            }
        }
    }
}
