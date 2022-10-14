using System.Text;
using System.Text.Json.Nodes;

namespace LangAPI
{
    public class API
    {
        private string secret = "7o8TTpNqESndMC6f4DzFTYgJCJ4GT6qG";
        private string URL = "https://api.apilayer.com/keyword";
        private HttpClient client = new HttpClient();
        private string[] Document {get;set;}

        public string[]? Keywords {get;set;}
        public API(string[] document)
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

        public class QuotaExceededException : Exception
        {
            public override string Message => "The request couldn't be completed because the API's quota limit has been reached.";
        }
    }
}