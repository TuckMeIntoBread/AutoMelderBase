using System.Linq;
using ff14bot.Managers;
using LlamaLibrary.JsonObjects;
using Newtonsoft.Json;

namespace AutoMelder.MeldingLogic
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Materia
    {
        [JsonProperty("RawItemId")]
        public uint RawItemId { get; }

        private MateriaItem _info;

        private int _index;

        public MateriaItem Info
        {
            get
            {
                if (_info == null)
                {
                    SetInfo();
                }

                return _info;
            }
        }

        public int Index
        {
            get
            {
                if (_index == 0)
                {
                    SetInfo();
                }

                return _index;
            }
        }

        public Item Item => DataManager.GetItem(RawItemId);

        public string ItemName => Item.CurrentLocaleName;

        public Materia()
        {
        }

        public Materia(uint rawItemId)
        {
            RawItemId = rawItemId;
            SetInfo();
        }

        private void SetInfo()
        {
            _info = LlamaLibrary.ResourceManager.MateriaList.Value.SelectMany(i => i.Value).FirstOrDefault(r => r.Key == RawItemId);
            _index = LlamaLibrary.ResourceManager.MateriaList.Value.FirstOrDefault(r => r.Value.Any(k=> k.Key == RawItemId)).Key;
        }

        public override string ToString()
        {
            if (RawItemId == 0) return "None";
            return Info.ToString();
        }

        public static readonly Materia Empty = new Materia(0);
    }
}