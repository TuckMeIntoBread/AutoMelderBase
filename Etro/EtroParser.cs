using System.Collections.Generic;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using ff14bot.Enums;
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
            foreach (MeldInfo meldInfo in meldRequest.AllMelds())
            {
                meldInfo.SetEtroInfo(etroResponse);
            }
            Log.Information("Parsed etro info.");

            return meldRequest;
        }

        public static void SetEtroInfo(this MeldInfo meld, JObject info)
        {
            JToken itemToken = info[meld.EtroKey()];
            if (itemToken == null || itemToken.Type == JTokenType.Null) return;
            
            // Safely handle potential null value
            uint? nullableItemId = itemToken.Value<uint?>();
            if (!nullableItemId.HasValue || nullableItemId.Value == 0) return;
            
            uint itemId = nullableItemId.Value;
            meld.ItemId = itemId;
            
            string materiaString;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (meld.EquipType)
            {
                case EquipmentSlot.Ring1:
                    materiaString = $"{itemId.ToString()}L";
                    break;
                case EquipmentSlot.Ring2:
                    materiaString = $"{itemId.ToString()}R";
                    break;
                default:
                    materiaString = itemId.ToString();
                    break;
            }
            
            JToken materiaInfo = info["materia"]?[materiaString];
            if (materiaInfo == null || !materiaInfo.HasValues) return;
            
            var materiaIds = new List<int>();
            foreach (JToken jToken in materiaInfo.Values())
            {
                // Safely handle potential null values in materia array
                int? materiaId = jToken.Value<int?>();
                if (materiaId.HasValue)
                {
                    materiaIds.Add(materiaId.Value);
                }
            }
            
            for (int i = 0; i < materiaIds.Count; i++)
            {
                MateriaItem materia = MateriaParser.GetMateriaFromId(materiaIds[i]);
                meld.SetSlot(i, materia);
            }
        }
    }
}
