using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using LlamaLibrary.Logging;
using Newtonsoft.Json.Linq;
using static AutoMelder.Ariyala.AriyalaParser;
using static AutoMelder.Etro.EtroParser;

namespace AutoMelder
{
    public static class MeldLinkParser
    {
        private static readonly LLogger Log = new LLogger("UriParser", Colors.Coral);
        
        public static readonly Regex AriyalaRegex = new Regex(@"(?<code>[A-Z0-9]{4,8})[\r\n\/]*$$");
        
        public static readonly Regex EtroRegex = new Regex(@"(?<code>[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})[\r\n\/]*$");

        public static MeldRequest ParseLinkOrId(string linkOrId)
        {
            Match etroMatch = EtroRegex.Match(linkOrId);
            if (etroMatch.Success)
            {
                return GetEtroMeldInfo(etroMatch.Groups["code"].Value);
            }
            Match ariyalaMatch = AriyalaRegex.Match(linkOrId);
            if (ariyalaMatch.Success)
            {
                return GetAriyalaMeldInfo(ariyalaMatch.Groups["code"].Value);
            }
            
            Log.Error($"Couldn't parse {linkOrId}!");
            return MeldRequest.Empty;
        }

        public static bool TryGetResponse(string uri, out JObject jObject)
        {
            jObject = null;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "text/json; charset=utf-8";
            HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error($"Couldn't get a response from {uri}. StatusCode: {response?.StatusCode}");
                return false;
            }

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                jObject = JObject.Parse(reader.ReadToEnd());
            }

            if (jObject == null || jObject.Count == 0)
            {
                Log.Error($"Couldn't parse the retrieved json. URI: {uri}");
                return false;
            }

            return true;
        }
    }
}