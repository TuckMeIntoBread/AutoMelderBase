using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using LlamaLibrary.Logging;
using Newtonsoft.Json.Linq;

namespace AutoMelder.AriyalaParser
{
    public static class AriyalaParser
    {
        private static readonly LLogger Log = new LLogger("AriyalaParser", Colors.Coral);
        private static readonly Regex CodeParser = new Regex(@"[A-Z0-9]{4,8}\/?$");
        
        public static MeldRequest GetAriyalaMeldInfo(string ariyalaCode)
        {
            string parsedCode = CodeParser.Match(ariyalaCode).Value;
            
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
            meldRequest.MainHand.SetInfo(info, "mainhand");
            meldRequest.OffHand.SetInfo(info, "offhand");
            meldRequest.Head.SetInfo(info, "head");
            meldRequest.Chest.SetInfo(info, "chest");
            meldRequest.Hands.SetInfo(info, "hands");
            meldRequest.Legs.SetInfo(info, "legs");
            meldRequest.Feet.SetInfo(info, "feet");
            meldRequest.Ears.SetInfo(info, "ears");
            meldRequest.Neck.SetInfo(info, "neck");
            meldRequest.Wrist.SetInfo(info, "wrist");
            meldRequest.RingLeft.SetInfo(info, "ringLeft");
            meldRequest.RingRight.SetInfo(info, "ringRight");

            return meldRequest;
        }
    }
}