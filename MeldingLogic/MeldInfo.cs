using System.Linq;
using System.Windows.Forms;
using AutoMelder.Ariyala;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.JsonObjects;
using Newtonsoft.Json.Linq;
using static AutoMelder.Extensions;

namespace AutoMelder.MeldingLogic
{
    public class MeldInfo
    {
        public string Type { get; }
        
        public uint ItemId { get; set; }

        public BagSlot EquipSlot => InventoryManager.GetBagByInventoryBagId(InventoryBagId.EquippedItems)[GetSlotByType(Type)];

        public string ItemName => DataManager.GetItem(ItemId)?.CurrentLocaleName;
        
        public MateriaItem Slot1 { get; set; }
        
        public MateriaItem Slot2 { get; set; }
        
        public MateriaItem Slot3 { get; set; }

        public MateriaItem Slot4 { get; set; }

        public MateriaItem Slot5 { get; set; }

        public void SetTextBoxes(Form settingsForm)
        {
            foreach (TextBox textBox in settingsForm.GetAllControls().OfType<TextBox>().Where(x => x.Name.StartsWith(Type) && x.Name.Contains("Materia")))
            {
                if (textBox.Name.EndsWith("1")) textBox.Text = Slot1?.ToRomanString() ?? "None";
                if (textBox.Name.EndsWith("2")) textBox.Text = Slot2?.ToRomanString() ?? "None";
                if (textBox.Name.EndsWith("3")) textBox.Text = Slot3?.ToRomanString() ?? "None";
                if (textBox.Name.EndsWith("4")) textBox.Text = Slot4?.ToRomanString() ?? "None";
                if (textBox.Name.EndsWith("5")) textBox.Text = Slot5?.ToRomanString() ?? "None";
            }
        }

        public bool IsItemMismatched() => ItemId != (EquipSlot?.RawItemId ?? 0);

        public void SetInfo(JToken info)
        {
            ItemId = info["items"][Type].Value<uint>();
            JToken materiaInfo = info["materiaData"][$"{Type}-{ItemId}"];
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

        public MeldInfo(string type)
        {
            Type = type;
        }
    }
}