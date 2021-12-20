using Newtonsoft.Json;

namespace AutoMelder.MeldingLogic
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MeldInfo
    {
        [JsonProperty("Slot1")]
        public Materia Slot1 { get; set; } = Materia.Empty;
        
        [JsonProperty("Slot2")]
        public Materia Slot2 { get; set; } = Materia.Empty;
        
        [JsonProperty("Slot3")]
        public Materia Slot3 { get; set; } = Materia.Empty;
        
        [JsonProperty("Slot4")]
        public Materia Slot4 { get; set; } = Materia.Empty;
        
        [JsonProperty("Slot5")]
        public Materia Slot5 { get; set; } = Materia.Empty;

        public MeldInfo()
        {
        }
    }
}