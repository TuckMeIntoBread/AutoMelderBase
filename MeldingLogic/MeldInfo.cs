using AutoMelder.AriyalaParser;
using LlamaLibrary.JsonObjects;
using Newtonsoft.Json.Linq;

namespace AutoMelder.MeldingLogic
{
    public class MeldInfo
    {
        public uint ItemId { get; set; }
        
        public MateriaItem Slot1 { get; set; }
        
        public MateriaItem Slot2 { get; set; }
        
        public MateriaItem Slot3 { get; set; }

        public MateriaItem Slot4 { get; set; }

        public MateriaItem Slot5 { get; set; }

        public void SetInfo(JToken info, string ariyalaName)
        {
            ItemId = info["items"][ariyalaName].Value<uint>();
            SetMateria(info["materiaData"]);
        }

        private void SetMateria(JToken materiaInfo)
        {
            if (materiaInfo == null || !materiaInfo.HasValues) return;
            string[] materiaArray = materiaInfo.Value<string[]>();
            for (int i = 0; i < materiaArray.Length; i++)
            {
                MateriaItem materia = MateriaParser.GetMateriaItem(materiaArray[i]);
                SetSlot(i, materia);
            }
        }

        private void SetSlot(int slot, MateriaItem materia)
        {
            switch (slot)
            {
                case 0:
                    Slot1 = materia;
                    break;
                case 1:
                    Slot2 = materia;
                    break;
                case 2:
                    Slot3 = materia;
                    break;
                case 3:
                    Slot4 = materia;
                    break;
                case 4:
                    Slot5 = materia;
                    break;
            }
        }
    }
}