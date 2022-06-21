using System;
using System.Web;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Net.Http.Json;

namespace imgfetch
{
    internal class Program
    {
        public static string[] APICalls = new string[] { "https://www.googleapis.com/customsearch/v1?q={0}&num=10&searchType=image&key=AIzaSyCszddNdBvhdD0NQPWN-D7sFBHIm0dVNBc&cx=9104bba6a696b497a&rights=(cc_publicdomain%7Ccc_attribute%7Ccc_sharealike%7Ccc_nonderived)", "https://pixabay.com/api/?key=25898419-03dcbee5c44442ba2477affd7&q={0}&image_type=photo"};

        enum Services
        {
            GoogleCSE,
            Pixabay
        }

        static void Main(string[] args)
        {
            //Teste de API de busca de imagens.
            Console.Clear();
            Console.Title = "Imgfetch Image API testing";
            Console.WriteLine("Welcome to Imgfetch!");
            Console.WriteLine("======================\n");

            services:
            Console.WriteLine("Services available:");
            Console.WriteLine("(1) Google Custom Search");
            Console.WriteLine("(2) Pixabay Search API");
            Console.Write("Which service? [ ]");
            
            Console.SetCursorPosition(16,6);
            char input = (char)Console.Read();
            Console.ReadKey();
            Console.WriteLine("\n");
            Services selectedService = Services.GoogleCSE;
            string api = "";
            if (input == '1')
            {
                selectedService = Services.GoogleCSE;
            }
            else if (input == '2')
            {
                selectedService = Services.Pixabay;
            }
            else 
            {
                goto services;
            }
            api = APICalls[(int)selectedService];
            Console.WriteLine("Selected service: " + selectedService.ToString());
            //Console.ReadKey();

            search:
            
            Console.Write("Please enter your search term: ");
            string? q = Console.ReadLine();
            if (q != null && q != "")
            {
                //make http request
                HttpClient client = new HttpClient();
                HttpRequestMessage m = new HttpRequestMessage();
                
                Console.Write("Defining parameters...");
                m.Method = HttpMethod.Get;
                m.RequestUri = new Uri(string.Format(api,q));
                Console.WriteLine("OK");
                
                Console.Write("Sending request to " + m.RequestUri.ToString() + "...");
                HttpResponseMessage result = client.Send(m);
                Console.WriteLine("OK");
                
                //parse contents
                if (result.Content != null)
                {
                    if (selectedService == Services.GoogleCSE)
                    {
                        Parse_GoogleCSE(result);
                    }
                    else if (selectedService == Services.Pixabay)
                    {
                        Parse_Pixabay(result);
                    }
                }
            }
            else
            {
                goto search;
            }
        }

        static void Parse_GoogleCSE(HttpResponseMessage result)
        {
            JsonNode obj = null;
            string title;
            int qcount = 0;
            Console.WriteLine("\nOutput from GCSE received------------------------:");
            try
            {
                obj = JsonObject.Parse(result.Content.ReadAsStream()).AsObject();
                title = (string)obj["queries"]["request"][0]["title"];
                qcount = (int)obj["queries"]["request"][0]["count"];
                Console.WriteLine("Title: \""+title+"\"\nQty of requested images: "+qcount+"\n\nImage URLs:");
            }
            catch
            {
                try
                {
                    obj = JsonObject.Parse(result.Content.ReadAsStream()).AsObject();
                    Console.Error.WriteLine("[!!] Failed to parse content! Possibly an issue with the output/service. See details: ");
                    Console.Error.WriteLine(obj.ToString());
                }
                catch
                {
                    Console.Error.WriteLine("Service not available/request malformed. Please check the code/internet connection/service availability.");
                }
            }

            for (int i = 0;i<qcount;i++)
            {
                try
                {
                    string url = (string)obj["items"][i]["link"];
                    Console.WriteLine((i+1) + " - " + url);
                }
                catch
                {
                    Console.WriteLine((i+1)+" - (Error reading URL)");
                    qcount--;
                    continue;
                }
            }
        }

        static void Parse_Pixabay(HttpResponseMessage result)
        {
            JsonNode obj = null;
            Console.WriteLine("\nOutput from Pixabay received------------------------:");
            try
            {
                obj = JsonObject.Parse(result.Content.ReadAsStream()).AsObject();
            }
            catch
            {
                Console.Error.WriteLine("Service not available/request malformed. Please check the code/internet connection/service availability.");
                return;
            }

            try
            {
                int i = 0;
                while (true) 
                {
                    string url, user;
                    int id;
                    url = (string)obj["hits"][i]["largeImageURL"];
                    user = (string)obj["hits"][i]["user"];
                    id = (int)obj["hits"][i]["id"];
                    
                    Console.WriteLine(i + " - (Image " + id + " by " + user+") " + url);
                    i++;
                }
            }
            catch
            {

            }
        }
    }
}