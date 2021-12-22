using System.Collections.Generic;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using LlamaLibrary.JsonObjects;
using LlamaLibrary.Logging;
using Newtonsoft.Json.Linq;

namespace AutoMelder.Etro
{
    public static class EtroParser
    {
        private static readonly LLogger Log = new LLogger("EtroParser", Colors.Coral);

        public static MeldRequest GetEtroMeldInfo(string etroCode)
        {
            Log.Information($"Parsing etro code {etroCode}");
            
            var uri = $"https://etro.gg/api/gearsets/{etroCode}/";
            if (!MeldLinkParser.TryGetResponse(uri, out JObject etroResponse)) return MeldRequest.Empty;

            MeldRequest meldRequest = new MeldRequest();
            foreach (MeldInfo meldInfo in meldRequest.GetAllMelds())
            {
                meldInfo.SetEtroInfo(etroResponse);
            }
            Log.Information("Parsed etro info.");

            return meldRequest;
        }

        public static void SetEtroInfo(this MeldInfo meld, JObject info)
        {
            uint itemId = info[meld.EtroKey()]?.Value<uint>() ?? 0;
            if (itemId == 0) return;
            meld.ItemId = itemId;
            JToken materiaInfo = info[itemId];
            if (materiaInfo == null || !materiaInfo.HasValues) return;
            var materiaIds = new List<uint>();
            foreach (JToken jToken in materiaInfo.Values())
            {
                materiaIds.Add(jToken.Value<uint>());
            }
            for (int i = 0; i < materiaIds.Count; i++)
            {
                MateriaItem materia = MateriaParser.GetMateriaFromId(materiaIds[i]);
                meld.SetSlot(i, materia);
            }
        }
    }
}