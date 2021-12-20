using Newtonsoft.Json;

namespace AutoMelder.MeldingLogic
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MeldRequest
    {
        [JsonProperty("MainHand")]
        public MeldInfo MainHand { get; } = new MeldInfo();

        [JsonProperty("Offhand")]
        public MeldInfo OffHand { get; } = new MeldInfo();

        [JsonProperty("Head")]
        public MeldInfo Head { get; } = new MeldInfo();

        [JsonProperty("Chest")]
        public MeldInfo Chest { get; } = new MeldInfo();

        [JsonProperty("Hands")]
        public MeldInfo Hands { get; } = new MeldInfo();

        [JsonProperty("Legs")]
        public MeldInfo Legs { get; } = new MeldInfo();

        [JsonProperty("Feet")]
        public MeldInfo Feet { get; } = new MeldInfo();

        [JsonProperty("Earring")]
        public MeldInfo Earring { get; } = new MeldInfo();

        [JsonProperty("Necklace")]
        public MeldInfo Necklace { get; } = new MeldInfo();

        [JsonProperty("Bracelet")]
        public MeldInfo Bracelet { get; } = new MeldInfo();

        [JsonProperty("Ring1")]
        public MeldInfo Ring1 { get; } = new MeldInfo();

        [JsonProperty("Ring2")]
        public MeldInfo Ring2 { get; } = new MeldInfo();

        public MeldRequest()
        {
        }
    }
}