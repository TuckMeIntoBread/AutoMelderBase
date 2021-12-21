using System;
using System.Collections.Generic;
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
                if (textBox.Name.EndsWith("1")) textBox.Text = Slot1?.ToFullString() ?? "None";
                if (textBox.Name.EndsWith("2")) textBox.Text = Slot2?.ToFullString() ?? "None";
                if (textBox.Name.EndsWith("3")) textBox.Text = Slot3?.ToFullString() ?? "None";
                if (textBox.Name.EndsWith("4")) textBox.Text = Slot4?.ToFullString() ?? "None";
                if (textBox.Name.EndsWith("5")) textBox.Text = Slot5?.ToFullString() ?? "None";
            }
        }

        public bool IsItemMismatched() => ItemId != (EquipSlot?.RawItemId ?? 0);

        public void SetInfo(JToken info)
        {
            if (info["items"][Type] == null) return;
            ItemId = info["items"][Type].Value<uint>();
            JToken materiaInfo = info["materiaData"][$"{Type}-{ItemId}"];
            if (materiaInfo == null || !materiaInfo.HasValues) return;
            var stringList = new List<string>();
            foreach (JToken stringToken in materiaInfo)
            {
                stringList.Add(stringToken.Value<string>());
            }
            for (int i = 0; i < stringList.Count; i++)
            {
                MateriaItem materia = MateriaParser.GetMateriaItem(stringList[i]);
                SetSlot(i, materia);
            }
        }

        private void SetSlot(int index, MateriaItem materia)
        {
            switch (index)
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
        
        public MateriaItem GetSlotByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return Slot1;
                case 1:
                    return Slot2;
                case 2:
                    return Slot3;
                case 3:
                    return Slot4;
                case 4:
                    return Slot5;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), $"{index} is out of range for materia slots! Valid range 0-4");
            }
        }

        public MeldInfo(string type)
        {
            Type = type;
        }
    }
}