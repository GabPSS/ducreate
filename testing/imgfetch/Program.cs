using System;
using System.Web;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Net.Http.Json;

namespace imgfetch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Teste de API de busca de imagens.
            Console.Clear();
            Console.Title = "Imgfetch Image API testing";
            Console.WriteLine("Welcome to Imgfetch!");
            Console.WriteLine("======================\n");
            search:
            
            string api = "https://www.googleapis.com/customsearch/v1?q={0}&num=10&searchType=image&key=AIzaSyCszddNdBvhdD0NQPWN-D7sFBHIm0dVNBc&cx=9104bba6a696b497a&rights=(cc_publicdomain%7Ccc_attribute%7Ccc_sharealike%7Ccc_nonderived)"; //URL da API. Substituir termo de busca por '{0}' para o string.Format depois
            
            Console.Write("Please enter your search term: ");
            string? q = Console.ReadLine();
            if (q != null)
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
                    Console.WriteLine("\nOutput received------------------------:");
                    try
                    {
                        var obj = JsonObject.Parse(result.Content.ReadAsStream()).AsObject();
                        string title = (string)obj["queries"]["request"][0]["title"];
                        int qcount = (int)obj["queries"]["request"][0]["count"];
                        Console.WriteLine("Title: \""+title+"\"\nQty of requested images: "+qcount+"\n\nImage URLs:");
                    }
                    catch
                    {
                        try
                        {
                            var obj = JsonObject.Parse(result.Content.ReadAsStream()).AsObject();
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
            }
            else
            {
                goto search;
            }
        }
    }
}