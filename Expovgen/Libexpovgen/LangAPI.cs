using System.Text;
using System.Text.Json.Nodes;

namespace Expovgen.LangAPI
{
    public class LangAPI1
    {
        private readonly string secret = "7o8TTpNqESndMC6f4DzFTYgJCJ4GT6qG";
        private readonly string URL = "https://api.apilayer.com/keyword";
        private HttpClient client = new HttpClient();
        private string[] Document { get; set; }

        public string[]? Keywords {get;set;}
        public string KeywordsFileTargetPath { get; set; } = "res\\keywords.txt";
        public LangAPI1(string[] document)
        {
            Document = document;
        }

        /// <summary>
        /// Obtains keywords from LangAPI Request
        /// </summary>
        /// <exception cref="QuotaExceededException"></exception>
        public void GetKeywords()
        {
            //Make request
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(URL);
            request.Headers.Add("apikey",secret);
            
            //Create document and add as content
            StringBuilder content = new StringBuilder();
            foreach (string line in Document)
            {
                content.Append(line + "\n");
            }
            byte[] contentBytes = Encoding.UTF8.GetBytes(content.ToString());
            request.Content = new ByteArrayContent(contentBytes);

            //Send and treat response
            HttpResponseMessage response = client.Send(request); 
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new QuotaExceededException();
            }

            //Parse JSON
            JsonNode? obj = null;
            List<string> keywords = new List<string>();
            try
            {
                obj = JsonObject.Parse(response.Content.ReadAsStream());
            }
            catch
            {
                Keywords = null;
            }

            //Extract keywords from JSON object
            if (obj is not null)
            {
                if (obj["result"] is not null)
                {
                    int count = 0;
                    while (true)
                    {
                        try
                        {
                            string keyword = (string)obj["result"][count]["text"];
                            keywords.Add(keyword is not null ? keyword : "");
                            count++;
                        }
                        catch
                        {
                            break;
                        }
                    }

                    List<string>? keywordsFinal = TreatKeywords(keywords.ToArray(), CondenseDocument(Document))?.ToList();
                    if (keywordsFinal == null)
                    {
                        Keywords = null;
                        keywordsFinal = new List<string>();
                    }
                    else
                    {
                        Keywords = keywordsFinal.ToArray();
                    }


                    Keywords = keywordsFinal.ToArray();
                }
                else
                {
                    Keywords = null;
                    
                }
            }
            else
            {
                Keywords = null;
            }

            File.WriteAllLines(KeywordsFileTargetPath, keywords);
        }

        /// <summary>
        /// Brings document into a single line
        /// </summary>
        public string CondenseDocument(string[] document)
        {
            string singleLineDocument = "";

            //Joins document together in one single line
            for (int i = 0; i < document.Length; i++)
            {

                string toAdd = document[i].Trim();
                if (toAdd == "")
                {
                    continue;
                }
                if (!toAdd.TrimEnd().EndsWith(".") && !(toAdd.TrimEnd().EndsWith('!') || toAdd.TrimEnd().EndsWith('?') || toAdd.TrimEnd().EndsWith(':') || toAdd.TrimEnd().EndsWith(';')))
                {
                    toAdd += ". ";
                }
                singleLineDocument += toAdd;
            }

            //returns it
            return singleLineDocument;
        }

        /// <summary>
        /// Treats an array of keywords based on a single line document string.
        /// </summary>
        /// <param name="keywords">the initial keyword array</param>
        /// <param name="singleLineDocument">a condensed document</param>
        public static string[]? TreatKeywords(string[] keywords, string singleLineDocument)
        {
            List<string> treatedKeywords = new();
            for (int i = 0; i < keywords.Length; i++)
            {
                string kw = keywords[i].Trim().ToLower();
                if (kw != "" && //Is not just whitespace or empty
                    !treatedKeywords.Contains(kw) && //Isn't repeated
                    singleLineDocument.Contains(kw) ) //Is contained within document
                {
                    treatedKeywords.Add(kw); //Add it to treatedKeywords
                }
            }
            if (treatedKeywords.Count > 0)
                return treatedKeywords.ToArray();
            else
                return null;
        }

        /// <summary>
        /// Creates document res\split.txt containing phrases divided by keyword (for showing image)
        /// </summary>
        public void SplitPhrases()
        {
            if (Keywords is not null)
            {
                string singleLineDocument = CondenseDocument(Document);

                //Creates new list of lookup indices
                List<int> lookupIndexes = new();

                //Creates an alternative document, used only for keyword lookup
                string lowerCaseSingleLineDocument = singleLineDocument.ToLower();
                
                //Iterates each keyword to find indexes for it, then add them to lookupIndexes
                foreach (string keyword in Keywords)
                {
                    //Creates new list of keyword indexes for $keyword
                    List<int> singleKeywordIndexes = new();
                    int x = 0;
                    do
                    {
                        //Looks for the first index after x (NOTE: This is where keywords should be treated before comparison!)
                        x = lowerCaseSingleLineDocument.IndexOf(keyword.Trim().ToLower(), x);
                        if (x != -1)
                        {
                            //If it found any, add to singleKeywordIndexes, then repeat
                            singleKeywordIndexes.Add(x);
                            x++;
                        }
                    } while (x != -1);

                    //Adds all indices for $keyword in lookupIndexes
                    lookupIndexes.AddRange(singleKeywordIndexes);
                }
                
                lookupIndexes.Sort();

                int counter = 0;
                foreach (int match in lookupIndexes)
                {
                    singleLineDocument = singleLineDocument.Insert(match + counter, "☼");
                    counter++;
                }

                //Splits the singleLineDocument into a string list by the ☼ character
                List<string> splitDocument = singleLineDocument.Split("☼").ToList();

                //Line treating so each one contains a single keyword
                for (int i = 0; i < splitDocument.Count; i++)
                {
                    bool containsAKeyword = false;

                    //Check if the line contains any keywords
                    foreach (string keyword in Keywords)
                    {
                        bool containsThisKeyword = splitDocument[i].Contains(keyword.Trim());
                        if (containsThisKeyword)
                        {
                            containsAKeyword = containsThisKeyword;
                            break;
                        }
                    }

                    //If it doesn't contain any keyword and it's not the last line, join it with tne next line
                    if (!containsAKeyword && i != splitDocument.Count - 1)
                    {
                        splitDocument[i + 1] = (splitDocument[i].TrimEnd() + " " + splitDocument[i + 1].TrimStart()).Trim();
                        splitDocument[i] = "";
                    }
                }

                //Go through each line to remove empty lines
                for (int i = 0; i < splitDocument.Count; i++)
                {
                    if (splitDocument[i].Trim() == "")
                    {
                        splitDocument.RemoveAt(i);
                        i--;
                        continue;
                    }
                    splitDocument[i] = splitDocument[i].Trim();
                }

                //Save to file and to document array
                Document = splitDocument.ToArray();
                File.WriteAllLines(@"res\split.txt",splitDocument);
            }

        }

        /// <summary>
        /// Makes res\cc.txt file
        /// </summary>
        public void MakeCaptions()
        {
            List<string> phrases = new();
            foreach (string line in Document)
            {
                char[] splitters = { '.',',',';',':','?','!' };
                
                List<int> Founds = new();
                foreach (char splitter in splitters)
                {
                    //Lopp through line in order to find splitter occurences and add their indexes to Founds
                    int x = 0;
                    do
                    {
                        x = line.IndexOf(splitter, x);
                        if (x != -1)
                        {
                            Founds.Add(x);
                            x++;
                        }
                    } while (x != -1);
                }
                Founds.Sort();

                List<string> splitUsingFounds = new();
                int start = 0;
                for (int i = 0; i <= Founds.Count; i++)
                {
                    int end = i != Founds.Count ? Founds[i] : (line.Length - 1);
                    int length = ((end - start) + 1);
                    splitUsingFounds.Add(line.Substring(start, length).Trim());
                    start += length;
                }

                for (int i = 0; i < splitUsingFounds.Count; i++)
                {
                    foreach (char splitter in splitters)
                    {
                        if (splitUsingFounds[i] == splitter.ToString())
                        {
                            splitUsingFounds[i - 1] += splitUsingFounds[i];
                            splitUsingFounds.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }

                phrases.AddRange(splitUsingFounds);
            }

            for (int i = 0; i < phrases.Count; i++)
            {
                if (phrases[i].Trim() == "")
                {
                    phrases.RemoveAt(i);
                    i--;
                }
            }

            Document = phrases.ToArray();
            File.WriteAllLines(@"res\cc.txt", phrases);
        }

        /// <summary>
        /// Adds identifier IDs to the start of the aligned res\splitcc.txt file
        /// </summary>
        /// <param name="keywordsFilePath">the res\keywords.txt file</param>
        /// <param name="filePath">the res\splitcc.txt file</param>
        public static void PrepareAlignmentAgainstKeywords(string keywordsFilePath, string filePath)
        {
            string[] doc = File.ReadAllLines(filePath);
            string[] Keywords = File.ReadAllLines(keywordsFilePath);

            //Checks each line and adds modifier id to the start of them
            for (int i = 0; i < doc.Length; i++)
            {
                for (int i1 = 0; i1 < Keywords.Length; i1++)
                {
                    if (doc[i].Contains(Keywords[i1].Trim()))
                    {
                        doc[i] = i1.ToString("000") + " " + doc[i];
                        break;
                    }
                }
                doc[i] = "000 " + doc[i];
            }

            File.WriteAllLines(filePath, doc);
        }

        public class QuotaExceededException : Exception
        {
            public override string Message => "The request couldn't be completed because the API's quota limit has been reached.";
        }
    }
}