using System.Collections.Generic;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using LlamaLibrary.JsonObjects;
using LlamaLibrary.Logging;
using Newtonsoft.Json.Linq;

namespace AutoMelder.Ariyala
{
    public static class AriyalaParser
    {
        private static readonly LLogger Log = new LLogger("AriyalaParser", Colors.Coral);

        public static MeldRequest GetAriyalaMeldInfo(string ariyalaCode)
        {
            Log.Information($"Parsing ariyala code {ariyalaCode}");

            var uri = $"https://ffxiv.ariyala.com/store.app?identifier={ariyalaCode}";
            if (!MeldLinkParser.TryGetResponse(uri, out JObject ariyalaResponse)) return MeldRequest.Empty;

            var dataset = ariyalaResponse["content"].Value<string>();
            JToken info = ariyalaResponse["datasets"][dataset.ToShortJobString()]?["normal"];
            if (info == null)
            {
                Log.Error($"Couldn't parse dataset {dataset}! Try using a different/newer ariyala link, or using an etro.gg link instead.");
                return MeldRequest.Empty;
            }
            MeldRequest meldRequest = new MeldRequest();
            foreach (MeldInfo meldInfo in meldRequest.AllMelds())
            {
                meldInfo.SetAriyalaInfo(info);
            }
            Log.Information("Parsed ariyala info.");

            return meldRequest;
        }
        
        public static void SetAriyalaInfo(this MeldInfo meld, JToken info)
        {
            if (info["items"][meld.AriyalaKey()] == null) return;
            meld.ItemId = info["items"][meld.AriyalaKey()].Value<uint>();
            JToken materiaInfo = info["materiaData"][$"{meld.AriyalaKey()}-{meld.ItemId}"];
            if (materiaInfo == null || !materiaInfo.HasValues) return;
            var stringList = new List<string>();
            foreach (JToken stringToken in materiaInfo)
            {
                stringList.Add(stringToken.Value<string>());
            }
            for (int i = 0; i < stringList.Count; i++)
            {
                MateriaItem materia = MateriaParser.GetMateriaFromAriyala(stringList[i]);
                meld.SetSlot(i, materia);
            }
        }
    }
}