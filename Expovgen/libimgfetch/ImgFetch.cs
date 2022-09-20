using System.Drawing;
using System.Text;
using System.Text.Json.Nodes;

namespace libimgfetch
{
    public enum Services { google, pixabay, pexels, rapidapi }

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
        #region Static API strings
        /// <summary>
        /// API endpoints list
        /// </summary>
        private static readonly string[] Endpoints = {
            "https://www.googleapis.com/customsearch/v1?q={0}&num=10&searchType=image&key=AIzaSyCszddNdBvhdD0NQPWN-D7sFBHIm0dVNBc&cx=9104bba6a696b497a",
            "https://pixabay.com/api/?key=25898419-03dcbee5c44442ba2477affd7&q={0}&image_type=photo",
            "https://api.pexels.com/v1/search?query={0}",
            "https://contextualwebsearch-websearch-v1.p.rapidapi.com/api/Search/ImageSearchAPI?q={0}&pageNumber=1&pageSize=10&autoCorrect=false"
        };

        /// <summary>
        /// Auth tokens and services
        /// TODO: Remove them from GH and make them local
        /// </summary>
        private static readonly string PexelsAuth = "563492ad6f917000010000012c20d17ed9954a579606dfed33e52735";
        private static readonly string RapidAPIAuth = "5e2741e3damsha64243e177061f0p15c2dajsn2195eab6c599";
        private static readonly string RapidAPISearchService1 = "contextualwebsearch-websearch-v1.p.rapidapi.com";
        #endregion

        //API properties
        public ImgFetchLogs Logs { get; set; } = new ImgFetchLogs();
        /// <summary>
        /// Defines the list of maximum donwloads permitted. To get as many as possible, set to -1
        /// </summary>
        public const int MaxDownloads = 1;
        private string[]? RequestURLs { get; set; }

        /// <summary>
        /// Formats a request url given a query and specified service
        /// </summary>
        /// <param name="service">The image search service</param>
        /// <param name="query">The search query</param>
        /// <returns>A formatted API request URL</returns>
        private static string GetURLString(Services service, string query)
        {
            
            string api = Endpoints[(int)service];
            if (service == Services.google || service == Services.pixabay || service == Services.pexels || service == Services.rapidapi)
            {
                api = string.Format(api, query);
            }
            return api;
        }

        #region Bitmap methods

        public List<Image> RequestImageBitmaps(ImgFetchRequest request)
        {
            (List<Stream> Files, List<List<string>> URLs) fileStreams = RequestImageStreams(request);
            return CheckImageBitmaps(fileStreams.Files, fileStreams.URLs);
        }

        ///// <summary>
        ///// Obtains image bitmaps from <see cref="RequestImageStreams(Services, string)"/>, then returns image streams as bitmaps (available on Windows only)
        ///// </summary>
        ///// <param name="service">The image search service</param>
        ///// <param name="query">The search query</param>
        ///// <returns>Returns an image list containing the returned bitmaps</returns>
        //private List<Image> RequestImageBitmaps(Services service, string query)
        //{
        //    (List<Stream> Files, List<string> URLs) fileStreams = RequestImageStreams(service, query);
        //    return CheckImageBitmaps(fileStreams.Files,fileStreams.URLs);
        //}

        ///// <summary>
        ///// Obtains image bitmaps from <see cref="RequestImageStreams(Services, string)"/>, then returns image streams as bitmaps (available on Windows only)
        ///// </summary>
        ///// <param name="service">The image search service</param>
        ///// <param name="query">The search query</param>
        ///// <param name="preferences">An <see cref="IServicePreferences"/> object representing preferences for the specified service</param>
        ///// <returns>Returns an image list containing the returned bitmaps</returns>
        //private List<Image> RequestImageBitmaps(Services service, string query, IServicePreferences preferences)
        //{
        //    (List<Stream> Files, List<string> URLs) fileStreams = RequestImageStreams(service, query, preferences);
        //    return CheckImageBitmaps(fileStreams.Files,fileStreams.URLs);
        //}

        /// <summary>
        /// Checks if streams provided are actually bitmaps, then returns a list of valid bitmaps (available on Windows only)
        /// </summary>
        /// <param name="fileStreams">The stream of files</param>
        /// <returns>An image list containing valid bitmaps</returns>
        private List<Image> CheckImageBitmaps(List<Stream> fileStreams, List<List<string>> URLs)
        {
            List<Image> bitmaps = new List<Image>();
            int extra_counter;
            int i = 0;
            foreach (Stream file in fileStreams)
            {
                extra_counter = 1;
                try
                {
                    Image img = Image.FromStream(file);
                    bitmaps.Add(img);
                }
                catch (Exception x)
                {
                    Logs.WriteLine("Warning while creating image from streams: " + x.Message);
                    Logs.WriteLine("Attempting to download extra image(s)...");
                    HttpClient emergency_downloader = new();
                    while (extra_counter < URLs.Count)
                    {
                        Stream? emImg = DownloadImage(emergency_downloader, URLs[i][extra_counter]);
                        extra_counter++;
                        if (emImg != null)
                        {
                            try
                            {
                                Image img = Image.FromStream(emImg);
                                bitmaps.Add(img);
                                break;
                            }
                            catch (Exception x2)
                            {
                                Logs.WriteLine("Warning while creating image from streams: " + x2.Message);
                            }
                        }
                    }
                }
                i++;
            }
            return bitmaps;
        }

        #endregion

        #region Stream requesting methods

        ///// <summary>
        ///// Repeats the same query using all available services and using default configuration values
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //[Obsolete]
        //private List<(List<Stream> Files, List<string> URLs)> RequestAllImageStreams(string query)
        //{
        //    List<(List<Stream> Files,List<string> URLs)> ImageStreams = new();
        //    int srv = 0;
        //    try
        //    {
        //        while (true)
        //        {
        //            Services service = (Services)srv;
        //            try
        //            {
        //                //ImageStreams.Add(RequestImageStreams(service, query));
        //            }
        //            catch (IndexOutOfRangeException)
        //            {
        //                break;
        //            }
        //            catch (Exception) { }
        //            srv++;
        //        }
        //    }
        //    catch { }
        //    return ImageStreams;
        //}

        /// <summary>
        /// Makes an ImgFetch web request using an ImgFetchRequest object
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public (List<Stream> Files, List<List<string>> URLs) RequestImageStreams(ImgFetchRequest request)
        {
            List<string> queries = request.SearchQueries;
            //bool pooledDownloads = request.EnablePoolDownloads;
            Services service = request.RequestingService;
            IServicePreferences? preferences = request.ServicePreferences;

            return RequestImageStreams(service, queries, preferences);
        }

        /// <summary>
        /// Sends a web request to <paramref name="service"/> with specified <paramref name="query"/>, processes the response, downloads response HTTP resources and returns their file streams
        /// WARNING: This method does not check if streams are actually images.
        /// </summary>
        /// <param name="service">The service to request images from</param>
        /// <param name="query">The search query</param>
        /// <returns>A List<Stream> containing all returned streams.</returns>
        private (List<Stream> Files, List<List<string>> URLs) RequestImageStreams(Services service, List<string> queries)
        {
            List<string> APIs = new();
            foreach (string query in queries)
            {
                string api = GetURLString(service, query);
                APIs.Add(api);
            }
            return RequestImageStreams(APIs, service); 
        }

        /// <summary>
        /// Sends a web request to <paramref name="service"/> with specified <paramref name="query"/> while passing down specific <paramref name="preferences"/>, processes the response, downloads response HTTP resources and returns their file streams. WARNING: This method does not check if streams are actually images.
        /// </summary>
        /// <param name="service">The service to request images from</param>
        /// <param name="query">The search query</param>
        /// <param name="preferences">An <see cref="IServicePreferences"/> object containing preferences for making the request</param>
        /// <returns>A List<Stream> containing all returned streams.</returns>
        private (List<Stream> Files, List<List<string>> URLs) RequestImageStreams(Services service, List<string> queries, IServicePreferences? preferences)
        {
            if (preferences is not null)
            {
                if (service == Services.google && preferences.GetType() == typeof(GooglePreferences))
                {
                    Logs.WriteLine("Setting request preferences...");
                    GooglePreferences? gpref = preferences as GooglePreferences;
                    if (gpref is not null) //It's always going to be...
                    {
                        List<string> APIs = new();
                        foreach (string query in queries)
                        {
                            string api = GetURLString(service, query);
                            if (gpref.UseCCLicense)
                            {
                                api += "&rights=(cc_publicdomain%7Ccc_attribute%7Ccc_sharealike%7Ccc_nonderived)";
                            }
                            APIs.Add(api);
                        }
                        return RequestImageStreams(APIs, service);
                    }
                }
                //NOTE: Add any other service preferences here
                
            }
            return RequestImageStreams(service, queries);
        }

        /// <summary>
        /// Makes an image request using a request URL, then downloads all streams it finds
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private (List<Stream> Files, List<List<string>> URLs) RequestImageStreams(List<string> requestUrl, Services service)
        {
            List<Stream> FileStreams = new List<Stream>();
            HttpClient downloader = new HttpClient();

            List<Thread> seach_threads = new();
            List<Thread> dl_threads = new List<Thread>();
            
            List<List<string>> ResultURLs = new();
            
            foreach (string query in requestUrl)
            {
                //try
                //{
                    Thread thread = new(() =>
                    {
                        ResultURLs.Add(MakeRequest(query, service));
                        Logs.WriteLine("Results for query '" + query + "'received");
                    });
                    thread.Start();
                    seach_threads.Add(thread);
                //} catch { }
            }
            for (int b = 0; b < seach_threads.Count; b++)
            {
                //try
                //{
                    if (seach_threads[b].IsAlive)
                    {
                        seach_threads[b].Join();
                    }
                //} catch { }
            }

            Logs.WriteLine("Begining file stream downloads...");
            while (ResultURLs.Count != requestUrl.Count)
            {

            }
            for (int x = 0; x < ResultURLs.Count; x++)
            {
                Stream? Download = DownloadImage(downloader, ResultURLs[x][0]);
                if (Download is not null)
                {
                    FileStreams.Add(Download);
                }
            }


            //for (int x = 0; x < ResultURLs.Count; x++)
            //{
            //    for (int i = 0; i != MaxDownloads; i++)
            //    {
            //        try
            //        {
            //            Thread thread = new Thread(() =>
            //            {
            //                int x_ = x;
            //                int i_ = i;
            //                Stream? download = DownloadImage(downloader, ResultURLs[x_][i_]);
            //                if (download is not null)
            //                {
            //                    FileStreams.Add(download);
            //                }
            //            });

            //            thread.Start();
            //            dl_threads.Add(thread);
            //        }
            //        catch
            //        {
            //            break;
            //        }
            //    }
            //}

            //Logs.WriteLine("All downloads started");
            //for (int i = 0; i < dl_threads.Count; i++)
            //{
            //    try
            //    {
            //        if (dl_threads[i].IsAlive)
            //        {
            //            dl_threads[i].Join();
            //        }
            //    }
            //    catch { }
            //}
            Logs.WriteLine("Stream download successful!");
            return (FileStreams,ResultURLs);
        }

        #endregion

        /// <summary>
        /// Uses the provided HttpClient to download an image from url
        /// </summary>
        /// <param name="downloader"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private Stream? DownloadImage(HttpClient downloader, object url)
        {
            try
            {
                Logs.WriteLine("Starting download for " + (string)url);
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri((string)url);
                request.Method = HttpMethod.Get;
                HttpResponseMessage returned = downloader.Send(request);
                //return returned.Content.ReadAsStream();
                Logs.WriteLine("Download of url " + url + " complete");
                return returned.Content.ReadAsStream();
            }
            catch (Exception x)
            {
                Logs.WriteLine("[Warning] while downloading: " + x.Message);
                return null;
            }
        }

        /// <summary>
        /// Interfaces with image APIs in order to get image download URLs
        /// </summary>
        /// <param name="requestUrl">The API endpoint</param>
        /// <param name="service">The API's endpoint</param>
        /// <returns>A string list containing the image URLs</returns>
        private List<string> MakeRequest(string requestUrl, Services service)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage m = new HttpRequestMessage();
            m.Method = HttpMethod.Get;
            m.RequestUri = new Uri(requestUrl);
            if (service == Services.pexels)
            {
                m.Headers.Add("Authorization", PexelsAuth);
            }
            if (service == Services.rapidapi)
            {
                m.Headers.Add("X-RapidAPI-Key", RapidAPIAuth);
                m.Headers.Add("X-RapidAPI-Host", RapidAPISearchService1);
            }
            Logs.WriteLine("Sending request...");
            HttpResponseMessage result = client.Send(m);
            if (result.IsSuccessStatusCode)
            {
                Logs.WriteLine("Parsing response...");
                //wait for it...
                if (result.Content != null)
                {
                    List<string> response = ParseJSONResponse(result, service);
                    RequestURLs = response.ToArray();
                    return response;
                }
                else
                {
                    Logs.WriteLine("WARNING: No output! Returning empty url list");
                }
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                Logs.WriteLine("Service returned Too Many Requests error: Quota is full");
            }
            RequestURLs = null;
            return new List<string>();
        }

        List<string> ParseJSONResponse(HttpResponseMessage result, Services service)
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

            switch (service)
            {
                case Services.google:
                    Parse_GoogleCSE(returning,obj);
                    break;
                case Services.pixabay:
                    Parse_Pixabay(returning,obj);
                    break;
                case Services.pexels:
                    Parse_Pexels(returning,obj);
                    break;
                case Services.rapidapi:
                    Parse_RapidAPI(returning,obj);
                    break;
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
        /// <summary>
        /// Method for JSON-parsing RapidAPI response
        /// </summary>
        /// <param name="returning"></param>
        /// <param name="obj"></param>
        private void Parse_RapidAPI(List<string> returning, JsonNode obj)
        {
            try
            {
                int i = 0;
                while (true)
                {
                    Logs.WriteLine("Parsing URL " + i + "...");
                    string url;
                    url = (string)obj["value"][i]["url"];
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
        public bool EnablePoolDownloads { get; set; } = false;
        public List<string> SearchQueries { get; set; } = new List<string>();
    }
}
