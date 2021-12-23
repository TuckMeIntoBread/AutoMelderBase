using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LlamaLibrary.JsonObjects;

namespace AutoMelder
{
    public static class MateriaParser
    {
        private static readonly Dictionary<string, string> AriyalaAttributeDictionary = new Dictionary<string, string>
        {
            { "STR", "Strength" },
            { "DEX", "Dexterity" },
            { "VIT", "Vitality" },
            { "INT", "Intelligence" },
            { "MND", "Mind" },
            { "PIE", "Piety" },
            { "HP", "HP" },
            { "MP", "MP" },
            { "TP", "TP" },
            { "GP", "GP" },
            { "CP", "CP" },
            { "PDMG", "Physical Damage" },
            { "MDMG", "Magic Damage" },
            { "DLY", "Delay" },
            { "BLKR", "Block Rate" },
            { "BLKS", "Block Strength" },
            { "TEN", "Tenacity" },
            { "ATK", "Attack Power" },
            { "DEF", "Defense" },
            { "DHT", "Direct Hit Rate" },
            { "EVA", "Evasion" },
            { "MDEF", "Magic Defense" },
            { "CRT", "Critical Hit" },
            { "SLASHING-RES", "Slashing Resistance" },
            { "PIERCING-RES", "Piercing Resistance" },
            { "BLUNT-RES", "Blunt Resistance" },
            { "AMP", "Attack Magic Potency" },
            { "HMP", "Healing Magic Potency" },
            { "FIRE-RES", "Fire Resistance" },
            { "ICE-RES", "Ice Resistance" },
            { "WIND-RES", "Wind Resistance" },
            { "EARTH-RES", "Earth Resistance" },
            { "LIGHTNING-RES", "Lightning Resistance" },
            { "WATER", "Water Resistance" },
            { "DET", "Determination" },
            { "SKS", "Skill Speed" },
            { "SPS", "Spell Speed" },
            { "MOR", "Morale" },
            { "CRFL-DESY", "Careful Desynthesis" },
            { "SLOW-RES", "Slow Resistance" },
            { "PETRIFICATION-RES", "Petrification Resistance" },
            { "PARALYSIS-RES", "Paralysis Resistance" },
            { "SILENCE-RES", "Silence Resistance" },
            { "BLIND-RES", "Blind Resistance" },
            { "POISON-RES", "Poison Resistance" },
            { "STUN-RES", "Stun Resistance" },
            { "SLEEP-RES", "Sleep Resistance" },
            { "BIND-RES", "Bind Resistance" },
            { "HEAVY-RES", "Heavy Resistance" },
            { "RDL", "Reduced Durability Loss" },
            { "ISG", "Increased Spiritbond Gain" },
            { "CMS", "Craftsmanship" },
            { "CRL", "Control" },
            { "GTH", "Gathering" },
            { "PCP", "Perception" },
        };

        private static readonly Regex AriyalaMateriaRegex = new Regex(@"(?<attribute>[^:]+):(?<tier>\d+)");

        private static Dictionary<int, List<MateriaItem>> MateriaList => LlamaLibrary.ResourceManager.MateriaList.Value;

        public static MateriaItem GetMateriaFromAriyala(string ariyalaMateriaString)
        {
            Match match = AriyalaMateriaRegex.Match(ariyalaMateriaString);
            string stat = AriyalaAttributeDictionary[match.Groups["attribute"].Value];
            int tier = int.Parse(match.Groups["tier"].Value);

            var statList = MateriaList.Values.First(x => x.Any(m => m.Stat == stat));
            MateriaItem specificMateria = statList.First(x => x.Tier == tier);
            return specificMateria;
        }

        public static MateriaItem GetMateriaFromId(int materiaId) => MateriaList.SelectMany(x => x.Value).First(x => x.Key == materiaId);
    }
}