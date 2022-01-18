using System;
using System.Linq;
using System.Windows.Forms;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.JsonObjects;

namespace AutoMelder.MeldingLogic
{
    public class MeldInfo
    {
        public EquipmentSlot EquipType { get; }
        
        public uint ItemId { get; set; }

        public BagSlot EquipSlot => InventoryManager.GetBagByInventoryBagId(InventoryBagId.EquippedItems)[EquipType];

        public string ItemName => DataManager.GetItem(ItemId)?.CurrentLocaleName;

        public bool IsEnabled(AutoMelderSettings settingsForm)
        {
            if (settingsForm.IgnoreMismatched && EquipSlot.RawItemId != ItemId) return false;
            return settingsForm.GetAllControls().OfType<CheckBox>().First(x => x.Name.StartsWith(EquipType.AriyalaKey()) && x.Name.EndsWith("EnabledCBox")).Checked;
        }

        public bool IsValid
        {
            get
            {
                if (ItemId == 0) return false;
                if (EquipSlot == null) return false;
                if (!EquipSlot.IsValid || !EquipSlot.IsFilled) return false;
                if (EquipSlot.RawItemId == 0) return false;
                if (EquipSlot.Item.MateriaSlots == 0) return false;
                return true;
            }
        }

        public MateriaItem Slot1 { get; set; }
        
        public MateriaItem Slot2 { get; set; }
        
        public MateriaItem Slot3 { get; set; }

        public MateriaItem Slot4 { get; set; }

        public MateriaItem Slot5 { get; set; }

        public void SetTextBoxes(Form settingsForm)
        {
            foreach (TextBox textBox in settingsForm.GetAllControls().OfType<TextBox>().Where(x => x.Name.StartsWith(EquipType.AriyalaKey()) && x.Name.Contains("Materia")))
            {
                if (textBox.Name.EndsWith("1")) textBox.Text = Slot1?.ToFullString() ?? "None";
                if (textBox.Name.EndsWith("2")) textBox.Text = Slot2?.ToFullString() ?? "None";
                if (textBox.Name.EndsWith("3")) textBox.Text = Slot3?.ToFullString() ?? "None";
                if (textBox.Name.EndsWith("4")) textBox.Text = Slot4?.ToFullString() ?? "None";
                if (textBox.Name.EndsWith("5")) textBox.Text = Slot5?.ToFullString() ?? "None";
            }
        }

        internal void SetSlot(int index, MateriaItem materia)
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
                    throw new ArgumentOutOfRangeException(nameof(index), index, $"{index} is out of range for materia slots! Valid range 0-4");
            }
        }

        public int GetIndexBySlot(MateriaItem item)
        {
            if (ReferenceEquals(item, Slot1)) return 0;
            if (ReferenceEquals(item, Slot2)) return 1;
            if (ReferenceEquals(item, Slot3)) return 2;
            if (ReferenceEquals(item, Slot4)) return 3;
            if (ReferenceEquals(item, Slot5)) return 4;
            throw new ArgumentOutOfRangeException(nameof(item), item, $"{item} didn't exist in {this}!");
        }

        public MeldInfo(EquipmentSlot equipType)
        {
            EquipType = equipType;
        }
    }
}