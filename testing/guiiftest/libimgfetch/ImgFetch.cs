using System.Drawing;
using System.Text;
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

    public class ImgFetchLogs
    {
        public List<string> Log = new List<string>();

        public class TextWrittenEventArgs : EventArgs
        {
            public string WrittenText { get; set; } = "";
        }

        public delegate void TextWrittenEventHandler(object source, TextWrittenEventArgs e);
        public event TextWrittenEventHandler TextWritten;

        protected virtual void OnTextWritten(string text)
        {
            if (TextWritten != null)
            {
                TextWritten(this, new TextWrittenEventArgs() { WrittenText = text });
            }
        }

        public void WriteLine(string text)
        {
            Log.Add(text);
            OnTextWritten(text);
        }
    }

    public class ImgFetch
    {
        /// <summary>
        /// API endpoints list
        /// </summary>
        private static readonly string[] Endpoints = {
            "https://www.googleapis.com/customsearch/v1?q={0}&num=10&searchType=image&key=AIzaSyCszddNdBvhdD0NQPWN-D7sFBHIm0dVNBc&cx=9104bba6a696b497a",
            "https://pixabay.com/api/?key=25898419-03dcbee5c44442ba2477affd7&q={0}&image_type=photo",
            "https://api.pexels.com/v1/search?query={0}"
        };

        /// <summary>
        /// Auth token -- specific to Pexels
        /// </summary>
        private static readonly string PexelsAuth = ""; //TODO: Get and add Pexels API authorization token

        public ImgFetchLogs Logs { get; set; } = new ImgFetchLogs();

        public ImgFetch() { }


        /// <summary>
        /// Formats a request url given a query and specified service
        /// </summary>
        /// <param name="service">The image search service</param>
        /// <param name="query">The search query</param>
        /// <returns>A formatted API request URL</returns>
        private static string GetURLString(Services service, string query)
        {
            
            string api = Endpoints[(int)service];
            if (service == Services.google || service == Services.pixabay || service == Services.pexels)
            {
                api = string.Format(api, query);
            }
            return api;
        }

        /// <summary>
        /// Obtains image bitmaps from <see cref="RequestImageStreams(Services, string)"/>, then returns image streams as bitmaps (available on Windows only)
        /// </summary>
        /// <param name="service">The image search service</param>
        /// <param name="query">The search query</param>
        /// <returns>Returns an image list containing the returned bitmaps</returns>
        public List<Image> RequestImageBitmaps(Services service, string query)
        {
            List<Stream> fileStreams = RequestImageStreams(service, query);
            return CheckImageBitmaps(fileStreams);
        }

        /// <summary>
        /// Obtains image bitmaps from <see cref="RequestImageStreams(Services, string)"/>, then returns image streams as bitmaps (available on Windows only)
        /// </summary>
        /// <param name="service">The image search service</param>
        /// <param name="query">The search query</param>
        /// <param name="preferences">An <see cref="IServicePreferences"/> object representing preferences for the specified service</param>
        /// <returns>Returns an image list containing the returned bitmaps</returns>
        public List<Image> RequestImageBitmaps(Services service, string query, IServicePreferences preferences)
        {
            List<Stream> fileStreams = RequestImageStreams(service, query, preferences);
            return CheckImageBitmaps(fileStreams);
        }

        /// <summary>
        /// Checks if streams provided are actually bitmaps, then returns a list of valid bitmaps (available on Windows only)
        /// </summary>
        /// <param name="fileStreams">The stream of files</param>
        /// <returns>An image list containing valid bitmaps</returns>
        private List<Image> CheckImageBitmaps(List<Stream> fileStreams)
        {
            List<Image> bitmaps = new List<Image>();
            foreach (Stream file in fileStreams)
            {
                try
                {
                    Image img = Bitmap.FromStream(file);
                    bitmaps.Add(img);
                }
                catch (Exception x)
                {
                    Logs.WriteLine("Warning while creating image from streams: " + x.Message);
                }
            }
            return bitmaps;
        }

        public List<Stream> RequestAllImageStreams(string query)
        {
            List<Stream> ImageStreams = new List<Stream>();
            int srv = 0;
            try
            {
                while (true)
                {
                    Services service = (Services)srv;
                    try
                    {
                        ImageStreams.AddRange(RequestImageStreams(service, query));
                    }
                    catch (IndexOutOfRangeException)
                    {
                        break;
                    }
                    catch (Exception) { }
                    srv++;
                }
            }
            catch { }
            return ImageStreams;
        }

        public List<Stream> RequestImageStreams(ImgFetchRequest request)
        {
            string query = request.SearchQuery;
            Services service = request.RequestingService;
            IServicePreferences? preferences = request.ServicePreferences;

            return RequestImageStreams(service, query, preferences);
        }

        /// <summary>
        /// Sends a web request to <paramref name="service"/> with specified <paramref name="query"/>, processes the response, downloads response HTTP resources and returns their file streams
        /// WARNING: This method does not check if streams are actually images.
        /// </summary>
        /// <param name="service">The service to request images from</param>
        /// <param name="query">The search query</param>
        /// <returns>A List<Stream> containing all returned streams.</returns>
        public List<Stream> RequestImageStreams(Services service, string query)
        {
            string api = GetURLString(service, query);
            return RequestImageStreams(api, service);
        }

        /// <summary>
        /// Sends a web request to <paramref name="service"/> with specified <paramref name="query"/> while passing down specific <paramref name="preferences"/>, processes the response, downloads response HTTP resources and returns their file streams. WARNING: This method does not check if streams are actually images.
        /// </summary>
        /// <param name="service">The service to request images from</param>
        /// <param name="query">The search query</param>
        /// <param name="preferences">An <see cref="IServicePreferences"/> object containing preferences for making the request</param>
        /// <returns>A List<Stream> containing all returned streams.</returns>
        public List<Stream> RequestImageStreams(Services service, string query, IServicePreferences? preferences)
        {
            if (preferences is not null)
            {
                if (service == Services.google && preferences.GetType() == typeof(GooglePreferences))
                {
                    Logs.WriteLine("Setting request preferences...");
                    GooglePreferences? gpref = preferences as GooglePreferences;
                    if (gpref is not null) //It's always going to be...
                    {
                        string api = GetURLString(service, query);
                        if (gpref.UseCCLicense)
                        {
                            api += "&rights=(cc_publicdomain%7Ccc_attribute%7Ccc_sharealike%7Ccc_nonderived)";
                        }
                        return RequestImageStreams(api, service);
                    }
                }
                //NOTE: Add any other service preferences here
                
            }
            return RequestImageStreams(service, query);
        }

        private List<Stream> RequestImageStreams(string requestUrl, Services service)
        {
            List<string> QueryURLs = RequestImageURLs(requestUrl, service);
            Logs.WriteLine("Begining file stream downloads..."); //TODO: I stopped here!
            List<Stream> FileStreams = new List<Stream>();
            foreach (string url in QueryURLs)
            {
                try
                {
                    Logs.WriteLine("Downloading stream from " + url);
                    HttpClient downloader = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Get;
                    HttpResponseMessage returned = downloader.Send(request);
                    FileStreams.Add(returned.Content.ReadAsStream());
                }
                catch (Exception x)
                {
                    Logs.WriteLine("[Warning] while downloading: " + x.Message);
                }
            }
            Logs.WriteLine("Stream download successful!");
            return FileStreams;
        }

        /// <summary>
        /// Interfaces with image APIs in order to get image URLs
        /// </summary>
        /// <param name="requestUrl">The API endpoint</param>
        /// <param name="service">The API's endpoint</param>
        /// <returns>A string list containing the image URLs</returns>
        private List<string> RequestImageURLs(string requestUrl, Services service)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage m = new HttpRequestMessage();
            m.Method = HttpMethod.Get;
            m.RequestUri = new Uri(requestUrl);
            if (service == Services.pexels)
            {
                m.Headers.Add("Authorization", PexelsAuth);
            }
            Logs.WriteLine("Sending request...");
            HttpResponseMessage result = client.Send(m);
            Logs.WriteLine("Parsing response...");
            //wait for it...
            if (result.Content != null)
            {
                return ParseResponse(result, service);
            }
            else
            {
                Logs.WriteLine("WARNING: No output! Returning empty url list");
                return new List<string>();
            }
        }

        List<string> ParseResponse(HttpResponseMessage result, Services service)
        {
            List<string> returning = new List<string>();
            JsonNode obj = null;
            try
            {
                obj = JsonObject.Parse(result.Content.ReadAsStream()).AsObject();
            }
            catch
            {
                return returning;
            }

            if (service == Services.google)
            {
                Parse_GoogleCSE(returning,obj);
            }
            else if (service == Services.pixabay)
            {
                Parse_Pixabay(returning,obj);
            }
            else if (service == Services.pexels)
            {
                Parse_Pexels(returning,obj);
            }
            
            return returning;
        }

        /// <summary>
        /// Method for JSON-parsing Google CSE response
        /// </summary>
        private void Parse_GoogleCSE(List<string> returning, JsonNode obj)
        {
            if (obj is not null)
            {
                if (obj["items"] is not null)
                {
                    int i = 0;
                    while (i <= 10)
                    {
                        try
                        {
                            Logs.WriteLine("Parsing URL "+i+"...");
                            string url;
                            url = (string)obj["items"][i]["link"];
                            returning.Add(url);
                            Logs.WriteLine(url);
                        }
                        catch
                        {
                            
                        }
                        finally
                        {
                            i++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method for JSON-parsing Google CSE response
        /// </summary>
        private void Parse_Pixabay(List<string> returning, JsonNode obj)
        {
            try
            {
                int i = 0;
                while (true)
                {
                    Logs.WriteLine("Parsing URL " + i + "...");
                    string url;
                    url = (string)obj["hits"][i]["largeImageURL"];
                    returning.Add(url);
                    Logs.WriteLine(url);
                    i++;
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Method for JSON-parsing Google CSE response
        /// </summary>
        private void Parse_Pexels(List<string> returning, JsonNode obj)
        {
            try
            {
                int i = 0;
                while (true)
                {
                    Logs.WriteLine("Parsing URL " + i + "...");
                    string url;
                    url = (string)obj["photos"][i]["src"]["original"];
                    returning.Add(url);
                    Logs.WriteLine(url);
                    i++;
                }
            }
            catch
            {

            }
        }
    }

    public class ImgFetchRequest
    {
        public Services RequestingService { get; set; } = Services.pixabay;
        public IServicePreferences? ServicePreferences { get; set; }
        public string SearchQuery { get; set; } = "";

        public ImgFetchRequest() { }
    }
}
