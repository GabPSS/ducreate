using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;

namespace imgfetch
{
    internal class Program
    {
        enum Services {
            google,
            googlecc,
            pixabay,
        }

        static string[] Endpoints = { 
            "https://www.googleapis.com/customsearch/v1?q={0}&num=10&searchType=image&key=AIzaSyCszddNdBvhdD0NQPWN-D7sFBHIm0dVNBc&cx=9104bba6a696b497a",
            "https://www.googleapis.com/customsearch/v1?q={0}&num=10&searchType=image&key=AIzaSyCszddNdBvhdD0NQPWN-D7sFBHIm0dVNBc&cx=9104bba6a696b497a&rights=(cc_publicdomain%7Ccc_attribute%7Ccc_sharealike%7Ccc_nonderived)",
            "https://pixabay.com/api/?key=25898419-03dcbee5c44442ba2477affd7&q={0}&image_type=photo"
        };
        static int Main(string[] args)
        {
            Services selectedService = Services.googlecc;
            try
            {
                for (int s = 0;;s++)
                {
                    if (args[0] == ((Services)s).ToString())
                    {
                        selectedService = (Services)s;
                        break;
                    }
                }
            }
            catch
            {
                Console.Error.WriteLine("Invalid service");
                return 0x01;
            }

            if (args.Length >= 2)
            {
                string query = "";
                try
                {
                    query = ConcatenateArgs(args);
                }
                catch
                {
                    Console.Error.WriteLine("Invalid query arguments");
                    return 0x02;
                }

                string api = string.Format(Endpoints[(int)selectedService],query);
                List<string> QueryURLs = MakeHTTPImageRequest(api,selectedService);
                string dirName = "_" + new Random().Next(0,11111111);
                Directory.CreateDirectory(dirName);
                Directory.SetCurrentDirectory(dirName);
                int i = 0;
                foreach (string url in QueryURLs)
                {
                    Console.WriteLine(url);
                    WebClient downloader = new WebClient();
                    downloader.DownloadFile(url,i.ToString("000"));
                    downloader.Dispose();
                    i++;
                }
                return 0;
            }
            else
            {
                Console.Error.WriteLine("Invalid arguments");
                return 0x03;
            }
        }

        static string ConcatenateArgs(string[] Args)
        {
            int startindex = 1;
            int endindex = Args.Length - 1;
            if (Args.Length >= 2)
            {
                string result = "";
                for (int i = startindex;i<endindex;i++)
                {
                    result += Args[i] + " ";
                }
                result += Args[endindex];

                return result;
            }
            else
            {
                return null;
            }
        }

        static List<string> MakeHTTPImageRequest(string api, Services service)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage m = new HttpRequestMessage();
            m.Method = HttpMethod.Get;
            m.RequestUri = new Uri(api);
            HttpResponseMessage result = client.Send(m);
            if (result.Content != null)
            {
                if (service == Services.google || service == Services.googlecc)
                {
                    return Parse_GoogleCSE(result);
                }
                else if (service == Services.pixabay)
                {
                    return Parse_Pixabay(result);
                }
                else
                {
                    return new List<string>();
                }
            }
            else
            {
                return new List<string>();
            }
        }

        static List<string> Parse_GoogleCSE(HttpResponseMessage result)
        {
            JsonNode obj = null;
            List<string> returning = new List<string>();
            int qcount = 0;
            try
            {
                obj = JsonObject.Parse(result.Content.ReadAsStream()).AsObject();
                qcount = (int)obj["queries"]["request"][0]["count"];
            }
            catch
            {
                return returning;
            }
            for (int i = 0;i<qcount;i++)
            {
                try
                {
                    string url = (string)obj["items"][i]["link"];
                    returning.Add(url);
                }
                catch
                {
                    continue;
                }
            }
            return returning;
        }

        static List<string> Parse_Pixabay(HttpResponseMessage result)
        {
            JsonNode obj = null;
            List<string> returning = new List<string>();
            try
            {
                obj = JsonObject.Parse(result.Content.ReadAsStream()).AsObject();
            }
            catch
            {
                return returning;
            }
            try
            {
                int i = 0;
                while (true) 
                {
                    string url;//, user;
                    //int id;
                    url = (string)obj["hits"][i]["largeImageURL"];
                    //user = (string)obj["hits"][i]["user"];
                    //id = (int)obj["hits"][i]["id"];
                    returning.Add(url);
                    i++;
                }
            }
            catch
            {

            }
            return returning;
        }
    }
}