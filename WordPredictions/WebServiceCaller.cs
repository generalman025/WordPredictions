using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace WordPredictions
{
    public class WebServiceCaller
    {
        private string url;
        private string token;

        public WebServiceCaller()
        {
            this.url = ConfigurationSettings.AppSettings["WebServiceUrl"];
            this.token = ConfigurationSettings.AppSettings["Token"];
        }

        public List<String> getWords(string word)
        {
            var enGBResult = callWebService("en-GB", word);
            if (enGBResult.Count > 0) return enGBResult;
            else{
                var daDK = callWebService("da-DK", word);
                return daDK;
            }
        }

        private List<String> callWebService(string language, string searchingWord)
        {
            try
            {
                var result = new List<String>();
                var request = WebRequest.CreateHttp(new Uri($"{url}?locale={language}&text=" + searchingWord));
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Method = "GET";

                using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    var body = sr.ReadToEnd();
                    var json = JArray.Parse(body);

                    int countItem = 0;
                    foreach (var item in json)
                    {
                        if (countItem == 10) break;
                        result.Add(item.ToString());
                        countItem++;
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
