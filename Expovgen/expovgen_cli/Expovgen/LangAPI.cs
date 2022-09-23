using System.Text;
using System.Text.Json.Nodes;

namespace Expovgen.LangAPI
{
    public class LangAPI1
    {
        private readonly string secret = "7o8TTpNqESndMC6f4DzFTYgJCJ4GT6qG";
        private readonly string URL = "https://api.apilayer.com/keyword";
        private HttpClient client = new HttpClient();
        private string[] Document {get;set;}

        public string[]? Keywords {get;set;}
        public LangAPI1(string[] document)
        {
            Document = document;
        }

        public void GetKeywords()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(URL);
            request.Headers.Add("apikey",secret);
            
            StringBuilder content = new StringBuilder();
            foreach (string line in Document)
            {
                content.Append(line + "\n");
            }
            byte[] contentBytes = Encoding.UTF8.GetBytes(content.ToString());
            request.Content = new ByteArrayContent(contentBytes);
            HttpResponseMessage response = client.Send(request);
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new QuotaExceededException();
            }
            JsonNode? obj = null;
            try
            {
                obj = JsonObject.Parse(response.Content.ReadAsStream());
            }
            catch
            {
                Keywords = null;
            }
            List<string> keywords = new List<string>();
            if (obj is not null)
            {
                int count = 0;
                while (true)
                {
                    try
                    {
                        string keyword = (string)obj["result"][count]["text"];
                        keywords.Add(keyword is not null ? keyword : "(null)");
                        count++;
                    }
                    catch
                    {
                        break;
                    }
                }
                Keywords = keywords.ToArray();
            }
            else
            {
                Keywords = null;
            }
        }

        public void SplitPhrases()
        {
            if (Keywords is not null)
            {
                string splitDocument = "";

                for (int i = 0; i < Document.Length; i++)
                {
                    string toAdd = Document[i].TrimStart();
                    if (!toAdd.TrimEnd().EndsWith(".") && !(toAdd.TrimEnd().EndsWith('!') || toAdd.TrimEnd().EndsWith('?') || toAdd.TrimEnd().EndsWith(':') || toAdd.TrimEnd().EndsWith(';')))
                    {
                        toAdd += ". ";
                    }
                    splitDocument += toAdd;
                }

                List<int> lookupIndexes = new();

                foreach (string keyword in Keywords)
                {
                    List<int> singleKeywordIndexes = new();

                    int x = 0;
                    do
                    {
                        x = splitDocument.IndexOf(keyword, x);
                        if (x != -1)
                        {
                            singleKeywordIndexes.Add(x);
                            x++;
                        }
                    } while (x != -1);

                    lookupIndexes.AddRange(singleKeywordIndexes);
                }
                
                lookupIndexes.Sort();

                int counter = 0;
                foreach (int match in lookupIndexes)
                {
                    splitDocument = splitDocument.Insert(match + counter, "§");
                    counter++;
                }

                List<string> brokenDocument = splitDocument.Split("§").ToList();

                for (int i = 0; i < brokenDocument.Count; i++)
                {
                    bool contains = false;
                    foreach (string keyword in Keywords)
                    {
                        bool doesContain = brokenDocument[i].Contains(keyword);
                        if (doesContain)
                        {
                            contains = doesContain;
                            break;
                        }
                    }

                    if (!contains && i != brokenDocument.Count - 1)
                    {
                        brokenDocument[i + 1] = (brokenDocument[i].TrimEnd() + " " + brokenDocument[i + 1].TrimStart()).Trim();
                        brokenDocument[i] = "";
                    }
                }

                for (int i = 0; i < brokenDocument.Count; i++)
                {
                    if (brokenDocument[i].Trim() == "")
                    {
                        brokenDocument.RemoveAt(i);
                        i--;
                        continue;
                    }

                    brokenDocument[i] = brokenDocument[i].Trim();
                }

                System.IO.File.WriteAllLines(@"res\align.txt",brokenDocument);
            }
        }
        public class QuotaExceededException : Exception
        {
            public override string Message => "The request couldn't be completed because the API's quota limit has been reached.";
        }
    }
}