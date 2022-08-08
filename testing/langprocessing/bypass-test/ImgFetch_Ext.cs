namespace libimgfetch
{
    public partial class ImgFetch
    {
        public Stream? RequestSingleImage(ImgFetchRequest request)
        {
            string query = request.SearchQuery;
            Services service = request.RequestingService;
            IServicePreferences? preferences = request.ServicePreferences;

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
                        return RequestSingleImageStream(api, service);
                    }
                }
                //NOTE: Add any other service preferences here
            }
            return null;
        }

        public Stream? RequestSingleImageStream(string requestUrl, Services service)
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
                    return returned.Content.ReadAsStream();
                }
                catch (Exception x)
                {
                    Logs.WriteLine("[Warning] while downloading: " + x.Message);
                }
            }
            return null;
        }

    }
}