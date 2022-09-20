using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace libimgfetch
{
    public class ImgFetch2
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

        #region API Properties

        // Public user-defined properties
        public ImgFetchLogs Logs { get; } = new ImgFetchLogs();
        public string[] RequestQueries { get; set;}
        public Services Service { get; set; }
        public IServicePreferences ServicePreferences { get; set; }
        
        // Private properties for use only within the API
        private List<(string query,string[] urls)>? ResultURLs { get; set; }
        private HttpClient webClient { get; set; } = new HttpClient();

        #endregion

        public ImgFetch2()
        {

        }

        private static string[] GetRequestStrings(Services service, string[] queries)
        {
            List<string> returning = new();
            foreach (string query in queries)
            {
                returning.Add(GetURLString(service, query));
            }
            return returning.ToArray();
        }

        private static string GetURLString(Services service, string query)
        {
            string api = Endpoints[(int)service];
            if (service == Services.google || service == Services.pixabay || service == Services.pexels || service == Services.rapidapi)
            {
                api = string.Format(api, query);
            }
            return api;
        }


        //TODO: Remove this later when it's not used
        [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
        sealed class WorkInProgressAttribute : Attribute
        {
            // See the attribute guidelines at 
            //  http://go.microsoft.com/fwlink/?LinkId=85236

            // This is a positional argument
            public WorkInProgressAttribute()
            {
                throw new NotImplementedException("The method used is currently being developed.");
            }
        }

        [WorkInProgress]
        public List<Image> RequestImageStreams()
        {
            //Variable definitions & attributions
            ResultURLs = new(); //Reset resultURLs
            List<string> requestURLs = new(); //URLs to feed into the Service's API.
            List<(string query, Stream downloadedImage)>? imageFiles = new(); //Corresponds to downloaded image file streams

            //Check service preferences & obtain requestURLs based on service preferences, if specified
            if (ServicePreferences is not null)
            {
                if (Service == Services.google && ServicePreferences.GetType() == typeof(GooglePreferences))
                {
                    Logs.WriteLine("Setting request preferences...");
                    GooglePreferences? gpref = ServicePreferences as GooglePreferences;
                    if (gpref is not null) //By consequence, it's never going to be, but the check is done anyway just in case
                    {
                        foreach (string query in RequestQueries)
                        {
                            string api = GetURLString(Service, query);
                            if (gpref.UseCCLicense)
                            {
                                api += "&rights=(cc_publicdomain%7Ccc_attribute%7Ccc_sharealike%7Ccc_nonderived)";
                            }
                            requestURLs.Add(api);
                        }
                    }
                }
                
                //^^^ NOTE: Add any other service preferences here ^^^

            }
            else
            {
                //Define variables manually
                requestURLs.AddRange(GetRequestStrings(Service, RequestQueries));
            }

            //Make requests and obtain Result URLs
            for (int i = 0;i < requestURLs.Count; i++)
            {
                GetSearchResultsRequest(requestURLs[i], RequestQueries[i], Service);
            }

            //Attempt to download returned images
            //x1 represents the query index.
            //x2 represents the indexes of the image URLs found for that query
            Stream? DownloadFirstImage;
            for (int x1 = 0; x1 < ResultURLs.Count; x1++)
            {
                DownloadFirstImage = DownloadImage(ResultURLs[x1].urls[0]);
            }

            //Next step: Check images for corrupted downloads and redownload any one of them that might have been missing.
            //IDEA: Make it so the program is able to return the non-downloaded image URLs, so new downloads can be requested
            return null;
        }

        /// <summary>
        /// Interfaces with image APIs in order to get image download URLs, and populate them into ResultURLs
        /// </summary>
        private void GetSearchResultsRequest(string requestUrl, string query, Services service)
        {
            //Declare HTTPClient
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(requestUrl);
            
            //Add service-specific request headers
            if (service == Services.pexels)
            {
                request.Headers.Add("Authorization", PexelsAuth);
            }
            if (service == Services.rapidapi)
            {
                request.Headers.Add("X-RapidAPI-Key", RapidAPIAuth);
                request.Headers.Add("X-RapidAPI-Host", RapidAPISearchService1);
            }
            
            //Send request
            Logs.WriteLine("Sending request...");
            HttpResponseMessage result = webClient.Send(request);
            if (result.IsSuccessStatusCode)
            {
                Logs.WriteLine("Parsing response...");
                if (result.Content != null)
                {
                    URLParser parser = new(Logs);
                    List<string> response = parser.ParseJSONResponse(result, service);
                    ResultURLs.Add((query, response.ToArray()));
                    return;
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
        }

        /// <summary>
        /// Uses the provided HttpClient to download an image from url
        /// </summary>
        /// <param name="downloader"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Stream? DownloadImage(string url)
        {
            try
            {
                Logs.WriteLine("Starting download for " + url);
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Get;
                HttpResponseMessage returned = webClient.Send(request);
                Logs.WriteLine("Download of url " + url + " complete");
                return returned.Content.ReadAsStream();
            }
            catch (Exception x)
            {
                Logs.WriteLine("[Warning] while downloading: " + x.Message);
                return null;
            }
        }

        #region URL parsing class + methods

        /// <summary>
        /// Semi-static methods for image parsing
        /// </summary>
        private class URLParser
        {
            private ImgFetchLogs Logs { get; set; }

            public URLParser(ImgFetchLogs logs)
            {
                Logs = logs;
            }

            public List<string> ParseJSONResponse(HttpResponseMessage result, Services service)
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
                        Parse_GoogleCSE(returning, obj);
                        break;
                    case Services.pixabay:
                        Parse_Pixabay(returning, obj);
                        break;
                    case Services.pexels:
                        Parse_Pexels(returning, obj);
                        break;
                    case Services.rapidapi:
                        Parse_RapidAPI(returning, obj);
                        break;
                }
                return returning;
            }

            /// <summary>
            /// Method for JSON-parsing Google CSE response
            /// </summary>
            public void Parse_GoogleCSE(List<string> returning, JsonNode obj)
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
                                Logs.WriteLine("Parsing URL " + i + "...");
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
            public void Parse_Pixabay(List<string> returning, JsonNode obj)
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
            public void Parse_Pexels(List<string> returning, JsonNode obj)
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
            public void Parse_RapidAPI(List<string> returning, JsonNode obj)
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
        
        #endregion
    }
}
