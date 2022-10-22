using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Expovgen;

namespace Expovgen.ImgFetch
{
    public enum Services { pixabay, rapidapi }
    public interface IServicePreferences { }
    
    
    public class ImgFetch2
    {
        #region Static API strings
        /// <summary>
        /// API endpoints list
        /// </summary>
        private static readonly string[] Endpoints = {

            "https://pixabay.com/api/?key=25898419-03dcbee5c44442ba2477affd7&q={0}&image_type=photo",
            "https://contextualwebsearch-websearch-v1.p.rapidapi.com/api/Search/ImageSearchAPI?q={0}&pageNumber=1&pageSize=10&autoCorrect=false"
        };

        /// <summary>
        /// Auth tokens and services
        /// TODO: Remove them from GH and make them local
        /// </summary>
        private static readonly string RapidAPIAuth = "75fe67d2d9mshe335b6b1ce66edap1dae17jsn6310432808ab";
        private static readonly string RapidAPISearchService1 = "contextualwebsearch-websearch-v1.p.rapidapi.com";
        #endregion

        #region API Properties

        // Public user-defined properties
        public ExpovgenLogs Logs { get; }
        public string[]? RequestQueries { get; set; }
        public Services Service { get; set; }
        public IServicePreferences? ServicePreferences { get; set; }
        public List<(string query, string[] urls)>? Results { get; set; }
        public (int Width,int Height) ImageDimensions { get; set; }
        public bool OverlayKeywordOnImage { get; set; } = false;

        // Private properties for use only within the API
        private HttpClient HttpFetchingClient { get; set; } = new HttpClient();
        //private (int width, int height) VideoDimensions { get; set; } = (1366, 768);

        #endregion

        /// <summary>
        /// Standard constructor for ImgFetch
        /// </summary>
        public ImgFetch2(ExpovgenLogs logs, (int Width, int Height) imageDimensions)
        {
            Logs = logs;
            ImageDimensions = imageDimensions;
        }

        #region Auxiliary methods for getting URL strings

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
            
            api = string.Format(api, query);
            
            return api;
        }

        #endregion

        /// <summary>
        /// Requests images from the selected service and returns a collection of downloaded image results
        /// 
        /// Requires first assigning Services, RequestQueries, and (if applicable) ServicePreferences
        /// </summary>
        public Image?[] RequestImages()
        {
            //Pre-process checks
            if (RequestQueries is null)
            {
                return Array.Empty<Image?>();
            }

            //Variable definitions & attributions
            Results = new(); //Reset resultURLs
            List<string> requestURLs = new(); //URLs to feed into the Service's API.

            //Check service preferences & obtain requestURLs based on service preferences, if specified
            //if (ServicePreferences is not null)
            {
                //if (Service == Services.google && ServicePreferences.GetType() == typeof(GooglePreferences))
                //{
                

                //^^^ NOTE: Add any other service preferences here ^^^

            }
            //else
            {
                //Define variables manually
                requestURLs.AddRange(GetRequestStrings(Service, RequestQueries));
            }

            //Make requests and obtain Result URLs
            for (int i = 0; i < requestURLs.Count; i++)
            {
                GetSearchResultsRequest(requestURLs[i], RequestQueries[i], Service);
            }

            //Attempt to download returned images
            //x1 represents the query's index.
            Stream?[] PossibleImageFiles = new Stream?[Results.Count];
            for (int x1 = 0; x1 < Results.Count; x1++)
            {
                try
                {
                    PossibleImageFiles[x1] = DownloadImage(Results[x1].urls[0]);
                }
                catch (ArgumentOutOfRangeException) { return Array.Empty<Image>(); }
                catch (IndexOutOfRangeException) { continue; }
            }

            //Convert each stream to a bitmap and redownload if missing.
            Image?[] Images = new Image?[Results.Count];
            for (int i = 0; i < Results.Count; i++)
            {
                // i represents the current image and query

                //Attempt to convert stream to image
                Image? convertedImage = null;
                if (PossibleImageFiles[i] != null)
                {
                    try
                    {
                        convertedImage = Image.FromStream(PossibleImageFiles[i]);
                        convertedImage = ResizeImage(convertedImage, ImageDimensions.Width, ImageDimensions.Height, Brushes.Black, OverlayKeywordOnImage, RequestQueries[i]);
                        goto loopend;
                    }
                    catch
                    {
                        Logs.WriteLine("Error while converting downloaded stream to image!");
                    }
                }

                //Download replacement image, as it couldn't get it previously
                if (Results[i].urls.Length > 1)
                {
                    for (int redownload = 1; redownload < Results[i].urls.Length; redownload++)
                    {
                        Logs.WriteLine("Attempting to download replacement image...");
                        Stream? possibleReplacementImage = DownloadImage(Results[i].urls[redownload]);
                        try
                        {
                            convertedImage = Image.FromStream(possibleReplacementImage);
                            break;
                        }
                        catch
                        {
                            Logs.WriteLine("Replacement image did not work");
                        }
                    }
                }

            loopend:
                Images[i] = convertedImage;
            }

            //Return the downloaded images
            return Images;

            //TODO: (IDEA) Make it so the program is able to return the non-downloaded image URLs, so new downloads can be requested
        }
        
        /// <summary>
        /// Rezises an image within the specified bounds of the height in an 16:9 aspect ratio
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="height"></param>
        /// <param name="backgroundFill"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Image ResizeImage(Image sourceImage, int tgtBaseWidth, int tgtBaseHeight, Brush backgroundFill, bool showKeyword = false, string? keyword = null)
        {
            //Size of the input image
            int srcWidth = sourceImage.Width;
            int srcHeight = sourceImage.Height;

            //Size of the image in the base
            int ImgHeight = tgtBaseHeight;
            int ImgWidth = tgtBaseHeight * srcWidth / srcHeight;

            //Image's position on the base
            int ImgPosX = (tgtBaseWidth - ImgWidth) / 2;
            int ImgPosY = 0;
                
            if (ImgPosX < 0)
            {
                ImgHeight = tgtBaseWidth * srcHeight / srcWidth;
                ImgWidth = tgtBaseWidth;

                ImgPosX = 0;
                ImgPosY = (tgtBaseHeight - ImgHeight) / 2;
            }

            //Creating a bitmap, the corresponding Graphics, and drawing the image
            Bitmap @base = new(tgtBaseWidth, tgtBaseHeight);
            Graphics gfx = Graphics.FromImage(@base);
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gfx.FillRectangle(backgroundFill, 0, 0, tgtBaseWidth, tgtBaseHeight);
            gfx.DrawImage(sourceImage, ImgPosX, ImgPosY, ImgWidth, ImgHeight);
            //Draw keyword text if asked to
            if (showKeyword)
            {
                Random rng = new();
                SizeF size;
                float fSize = 128;
                do
                {
                    fSize--;
                    size = gfx.MeasureString(keyword, new Font("Comic Sans MS", fSize));
                    if (fSize <= 1)
                    {
                        break;
                    }
                } while ((size.Width > (tgtBaseWidth * 0.7) || size.Height > (tgtBaseHeight * 0.3)));

                float posx = (float)((tgtBaseWidth - size.Width) * 0.5);
                float posy = (float)((tgtBaseHeight - size.Height) * 0.75);

                gfx.DrawString(keyword, new Font("Comic Sans MS", fSize), Brushes.Black, posx + 2, posy + 2);
                gfx.DrawString(keyword, 
                    new Font("Comic Sans MS", fSize), 
                    new SolidBrush(Color.FromArgb(rng.Next(200, 230), rng.Next(200, 230), rng.Next(200, 230))),
                    posx,
                    posy);
            }
            gfx.Dispose();

            //Returning the bitmap contianing the image
            return @base;

        }

        #region API-interfacing and image downloading methods

        /// <summary>
        /// Interfaces with image APIs in order to get image download URLs, and populate them into Results
        /// </summary>
        private void GetSearchResultsRequest(string requestUrl, string query, Services service)
        {
            //Declare HTTPClient
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUrl)
            };

            //Add service-specific request headers
            if (service == Services.rapidapi)
            {
                request.Headers.Add("X-RapidAPI-Key", RapidAPIAuth);
                request.Headers.Add("X-RapidAPI-Host", RapidAPISearchService1);
            }

            //Send request
            Logs.WriteLine("Sending request...");
            HttpResponseMessage result = HttpFetchingClient.Send(request);
            if (result.IsSuccessStatusCode)
            {
                Logs.WriteLine("Parsing response...");
                if (result.Content != null)
                {
                    URLParser parser = new(Logs);
                    List<string> response = parser.ParseJSONResponse(result, service);
                    Results.Add((query, response.ToArray()));
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
                HttpRequestMessage request = new()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                HttpResponseMessage returned = HttpFetchingClient.Send(request);
                Logs.WriteLine("Download of url " + url + " complete");
                return returned.Content.ReadAsStream();
            }
            catch (Exception x)
            {
                Logs.WriteLine("[Warning] while downloading: " + x.Message);
                return null;
            }
        }

        #endregion

        #region URL parsing class + methods

        /// <summary>
        /// Semi-static methods for image parsing
        /// </summary>
        private class URLParser
        {
            private ExpovgenLogs Logs { get; set; }

            public URLParser(ExpovgenLogs logs)
            {
                Logs = logs;
            }

            public List<string> ParseJSONResponse(HttpResponseMessage result, Services service)
            {
                List<string> returning = new();
                JsonNode obj;
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
                    case Services.pixabay:
                        Parse_Pixabay(returning, obj);
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
