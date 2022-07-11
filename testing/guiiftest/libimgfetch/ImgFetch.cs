using System.Text.Json.Nodes;

namespace libimgfetch
{
    public enum Services { google, pixabay, pexels }

    public interface IServicePreferences { }

    public class GooglePreferences : IServicePreferences
    {
        public bool UseCCLicense { get; set; }
        public GooglePreferences() { }
    }

    public class ImgFetch
    {
        private static string[] Endpoints = {
            "https://www.googleapis.com/customsearch/v1?q={0}&num=10&searchType=image&key=AIzaSyCszddNdBvhdD0NQPWN-D7sFBHIm0dVNBc&cx=9104bba6a696b497a",
            "https://pixabay.com/api/?key=25898419-03dcbee5c44442ba2477affd7&q={0}&image_type=photo",
            "https://api.pexels.com/v1/search?query={0}"
        };

        private static string PexelsAuth = ""; //TODO: Get and add Pexels API authorization token

        static string GetURLString(Services service, string query)
        {
            string api = Endpoints[(int)service];
            if (service == Services.google || service == Services.pixabay || service == Services.pexels)
            {
                api = string.Format(api, query);
            }
            return api;
        }

        /// <summary>
        /// Sends an HTTP request to the selected service and returns the file streams returned by the request.
        /// </summary>
        public static List<Stream> RequestImageStreams(Services service, string query)
        {
            string api = GetURLString(service,query);
            return RequestImageStreams(api, service);
        }

        public static List<Stream> RequestImageStreams(Services service, string query, IServicePreferences preferences)
        {
            if (service == Services.google && preferences.GetType() == typeof(GooglePreferences))
            {
                GooglePreferences gpref = preferences as GooglePreferences;
                string api = GetURLString(service,query);
                if (gpref.UseCCLicense)
                {
                    api += "&rights=(cc_publicdomain%7Ccc_attribute%7Ccc_sharealike%7Ccc_nonderived)";
                }
                return RequestImageStreams(api, service);
            }
            else
            {
                return new List<Stream>();
            }
        }

        public static List<Stream> RequestImageStreams(string requestUrl, Services service)
        {
            List<string> QueryURLs = RequestImageURLs(requestUrl, service);
            List<Stream> FileStreams = new List<Stream>();
            foreach (string url in QueryURLs)
            {
                try
                {
                    Console.WriteLine(url);
                    HttpClient downloader = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Get;
                    HttpResponseMessage returned = downloader.Send(request);
                    FileStreams.Add(returned.Content.ReadAsStream());
                }
                catch (Exception x)
                {
                    Console.Error.Write("[Warning] while downloading: " + x.Message);
                }
            }
            return FileStreams;
        }

        /// <summary>
        /// Interfaces with image APIs in order to get image URLs
        /// </summary>
        /// <param name="requestUrl">The API endpoint</param>
        /// <param name="service">The API's endpoint</param>
        /// <returns>A string list containing the image URLs</returns>
        static List<string> RequestImageURLs(string requestUrl, Services service)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage m = new HttpRequestMessage();
            m.Method = HttpMethod.Get;
            m.RequestUri = new Uri(requestUrl);
            if (service == Services.pexels)
            {
                m.Headers.Add("Authorization", PexelsAuth);
            }
            HttpResponseMessage result = client.Send(m);

            if (result.Content != null)
            {
                if (service == Services.google)
                {
                    return Parse_GoogleCSE(result);
                }
                else if (service == Services.pixabay)
                {
                    return Parse_Pixabay(result);
                }
                else if (service == Services.pexels)
                {
                    return Parse_Pexels(result);
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
            for (int i = 0; i < qcount; i++)
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
                    string url;
                    url = (string)obj["hits"][i]["largeImageURL"];
                    returning.Add(url);
                    i++;
                }
            }
            catch
            {

            }
            return returning;
        }

        static List<string> Parse_Pexels(HttpResponseMessage result)
        {
            throw new NotImplementedException();
        }
    }
}
