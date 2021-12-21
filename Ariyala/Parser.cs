using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using LlamaLibrary.Logging;
using Newtonsoft.Json.Linq;

namespace AutoMelder.Ariyala
{
    public static class Parser
    {
        private static readonly LLogger Log = new LLogger("AriyalaParser", Colors.Coral);
        private static readonly Regex CodeParser = new Regex(@"[A-Z0-9]{4,8}\/?$");
        
        public static MeldRequest GetAriyalaMeldInfo(string ariyalaCode)
        {
            if (!CodeParser.IsMatch(ariyalaCode))
            {
                Log.Error($"Couldn't parse {ariyalaCode} as an ariyala ID.");
                return new MeldRequest();
            }
            string parsedCode = CodeParser.Match(ariyalaCode).Value;
            Log.Information($"Parsing ariyala code {parsedCode}");
            
            var uri = $"https://ffxiv.ariyala.com/store.app?identifier={parsedCode}";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "text/json; charset=utf-8";
            HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error($"Couldn't get a response from {uri}. StatusCode: {response?.StatusCode}");
                return new MeldRequest();
            }

            JObject ariyalaResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                ariyalaResponse = JObject.Parse(reader.ReadToEnd());
            }

            if (ariyalaResponse == null || ariyalaResponse.Count == 0)
            {
                Log.Error($"Couldn't parse the retrieved json. URI: {uri}");
                return new MeldRequest();
            }

            var dataset = ariyalaResponse["content"].Value<string>();
            JToken info = ariyalaResponse["datasets"][dataset]["normal"];
            MeldRequest meldRequest = new MeldRequest();
            meldRequest.SetAllInfo(info);
            Log.Information("Parsed ariyala info.");

            return meldRequest;
        }
    }
}