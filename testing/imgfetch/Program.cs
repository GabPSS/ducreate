using System;
using System.Web;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Net.Http.Json;

namespace imgfetch
{
    internal class Program
    {
        public static string[] APICalls = new string[] { "https://www.googleapis.com/customsearch/v1?q={0}&num=10&searchType=image&key=AIzaSyCszddNdBvhdD0NQPWN-D7sFBHIm0dVNBc&cx=9104bba6a696b497a", "https://pixabay.com/api/?key=25898419-03dcbee5c44442ba2477affd7&q={0}&image_type=photo"};
        public static string RightsAddonGoogleCSE = "&rights=(cc_publicdomain%7Ccc_attribute%7Ccc_sharealike%7Ccc_nonderived)";
        enum Services
        {
            googlecse,
            pixabay
        }

        enum licenses
        {
            any,cc
        }

        static void Main(string[] args)
        {
            //Teste de API de busca de imagens.
            Services service = Services.googlecse;

            if (args.Length == 0)
            {
                CLI_Guide();
            }
            else if (args.Length == 1)
            {
                MakeHTTPRequest(APICalls[(int)service],args[0],service);
            }
            // else if (args.Contains("--help"))
            // {
            //     Console.WriteLine("imgfetch Image API testing program for EXPOTEC 2022\nUsage: imgfetch [-s {service} [-l {any:cc}] {query}]\n\n(no args)   Launch program in Guide mode\n-s          Service to use for the search (check guide mode for available services)\n-l          Filter images by license (available on select services like googlecse)\n            Types:\n                any - Any image type\n                cc - Creative commons licenses for commercial use\nquery       The search term\n");
            //     /*
            //     imgfetch Image API testing program for EXPOTEC 2022
            //     Usage: imgfetch [-s {service} [-l {any:cc}] {query}]

            //     (no args)   Launch program in Guide mode
            //     -s          Service to use for the search (check guide mode for available services)
            //     -l          Filter images by license (available on select services like googlecse)
            //                 Types:
            //                     any - Any image type
            //                     cc - Creative commons licenses for commercial use
            //     query       The search term
                
            //     */
            // }
            // else if (args.Length == 1 || args.Length > 3)
            // {
            //     Console.Error.WriteLine("Invalid arguments. Please check --help");
            //     return;
            // }
            // else
            // {
            //     if (args[0].StartsWith("-s "))
            //     {
            //         string i1 = args[0].Substring(3);
            //         if (i1 == "googlecse")
            //         {
            //             service = Services.googlecse;
            //         }
            //         else if (i1 == "pixabay")
            //         {
            //             service = Services.pixabay;
            //         }
            //         else
            //         {
            //             Console.Error.WriteLine("Invalid service. Please check --help or imgfetch (without args)");
            //             return;
            //         }
            //         //Path 1: -l and 3 args
            //         //Path 2: query and 2 args

            //         if (args.Length == 3 && args[1].StartsWith("-l "))
            //         {
            //             string i2 = args[1].Substring(3);
            //             if (i2 == "any")
            //             {
            //                 license = licenses.any;
            //             }
            //             else if (i2 == "cc" && service == Services.googlecse)
            //             {
            //                 license = licenses.cc;
            //             }
            //             else
            //             {
            //                 Console.Error.WriteLine("License type unknown or unsupported by the selected service. Please check --help");
            //                 return;
            //             }
            //             query = args[2];
            //         }
            //         else if (args.Length == 2)
            //         {
            //             query = args[1];
            //         }
            //         else
            //         {
            //             Console.Error.WriteLine("Invalid arguments. Please check --help");
            //             return;
            //         }

            //         //All OK at this point
            //         string api = APICalls[(int)service];
            //         if (service == Services.googlecse)
            //         {
            //             if (license == licenses.cc)
            //             {
            //                 api += RightsAddonGoogleCSE;
            //             }
            //         }
            //         MakeHTTPRequest(api,query,service);
            //     }
            //     else
            //     {
            //         Console.Error.WriteLine("Invalid service. Please check --help");
            //         return;
            //     }
            // }
        }

        static void CLI_Guide()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = "Imgfetch Image API testing";
            Console.WriteLine("Welcome to Imgfetch!");
            Console.WriteLine("======================\n");
            Console.ForegroundColor = ConsoleColor.White;
            
            services:
            Console.WriteLine("Services available:");
            Console.Write(" (A) ");Console.ForegroundColor = ConsoleColor.Blue;Console.WriteLine("Google Custom Search (googlecse)");Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" (B) ");Console.ForegroundColor = ConsoleColor.Green;Console.WriteLine("Pixabay Search API (pixabay)");Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Which service? ");
            
            
            ConsoleKey input = Console.ReadKey().Key;
            Console.WriteLine("\n");
            Services selectedService = Services.googlecse;
            string api = "",addon = "";
            if (input == ConsoleKey.A)
            {
                selectedService = Services.googlecse;
                Console.ForegroundColor = ConsoleColor.Cyan;
                license:
                Console.WriteLine("Select Image License type:");
                Console.WriteLine(" (A) Any image");
                Console.WriteLine(" (B) Creative Commons licenses for commercial use");
                Console.Write("Which type? ");
                ConsoleKeyInfo input2 = Console.ReadKey();
                if (input2.Key == ConsoleKey.A)
                {
                    addon = "";
                }
                else if (input2.Key == ConsoleKey.B)
                {
                    addon = RightsAddonGoogleCSE;
                }
                else
                {
                    goto license;
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n");
            }
            else if (input == ConsoleKey.B)
            {
                selectedService = Services.pixabay;
            }
            else 
            {
                goto services;
            }
            api = APICalls[(int)selectedService] + addon;
            Console.WriteLine("Selected service: " + selectedService.ToString());

            search:
            
            Console.Write("Please enter your search term: ");
            string? q = Console.ReadLine();
            if (q != null && q != "")
            {
                //make http request
                MakeHTTPRequest(api,q,selectedService);
            }
            else
            {
                goto search;
            }
        }

        static void MakeHTTPRequest(string api, string q, Services selectedService)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage m = new HttpRequestMessage();
            
            Console.Write("Defining parameters...");
            m.Method = HttpMethod.Get;
            m.RequestUri = new Uri(string.Format(api,q));
            Console.WriteLine("OK");
            
            Console.Write("Sending request to <");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(m.RequestUri.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(">...");

            HttpResponseMessage result = client.Send(m);
            Console.WriteLine("OK");
            
            //parse contents
            if (result.Content != null)
            {
                if (selectedService == Services.googlecse)
                {
                    Parse_GoogleCSE(result);
                }
                else if (selectedService == Services.pixabay)
                {
                    Parse_Pixabay(result);
                }
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