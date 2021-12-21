using System;
using System.Text;
using System.Windows.Forms;
using AutoMelder.MeldingLogic;
using ff14bot.Enums;
using ff14bot.Managers;

namespace AutoMelder
{
    public partial class AutoMelderSettings : Form
    {
        public AutoMelderSettings()
        {
            InitializeComponent();
        }

        public MeldRequest MeldRequest = new MeldRequest();

        private void ImportButton_Click(object sender, EventArgs e)
        {
            MeldRequest = Ariyala.Parser.GetAriyalaMeldInfo(ariyalaCodeBox.Text);
            MeldRequest.SetAllTextBoxes(this);
            Refresh();
            CheckItemMismatch();
            // TODO: Add check for materia mismatch.
        }

        private void CheckItemMismatch()
        {
            StringBuilder sb = new StringBuilder();
            Bag equipSlots = InventoryManager.GetBagByInventoryBagId(InventoryBagId.EquippedItems);
            if (MeldRequest.MainHand?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"MainHand Mismatch: {MeldRequest.MainHand.ItemName} - {equipSlots[EquipmentSlot.MainHand]?.Name}");
            }
            if (MeldRequest.OffHand?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"OffHand Mismatch: {MeldRequest.OffHand.ItemName} - {equipSlots[EquipmentSlot.OffHand]?.Name}");
            }
            if (MeldRequest.Head?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"Head Mismatch: {MeldRequest.Head.ItemName} - {equipSlots[EquipmentSlot.Head]?.Name}");
            }
            if (MeldRequest.Chest?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"Chest Mismatch: {MeldRequest.Chest.ItemName} - {equipSlots[EquipmentSlot.Body]?.Name}");
            }
            if (MeldRequest.Hands?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"Hands Mismatch: {MeldRequest.Hands.ItemName} - {equipSlots[EquipmentSlot.Hands]?.Name}");
            }
            if (MeldRequest.Legs?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"Legs Mismatch: {MeldRequest.Legs.ItemName} - {equipSlots[EquipmentSlot.Legs]?.Name}");
            }
            if (MeldRequest.Feet?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"Feet Mismatch: {MeldRequest.Feet.ItemName} - {equipSlots[EquipmentSlot.Feet]?.Name}");
            }
            if (MeldRequest.Ears?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"Ears Mismatch: {MeldRequest.Ears.ItemName} - {equipSlots[EquipmentSlot.Earring]?.Name}");
            }
            if (MeldRequest.Neck?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"Neck Mismatch: {MeldRequest.Neck.ItemName} - {equipSlots[EquipmentSlot.Necklace]?.Name}");
            }
            if (MeldRequest.Wrist?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"Wrist Mismatch: {MeldRequest.Wrist.ItemName} - {equipSlots[EquipmentSlot.Bracelet]?.Name}");
            }
            if (MeldRequest.RingLeft?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"RingLeft Mismatch: {MeldRequest.RingLeft.ItemName} - {equipSlots[EquipmentSlot.Ring1]?.Name}");
            }
            if (MeldRequest.RingRight?.IsItemMismatched() ?? false)
            {
                sb.AppendLine($"RingRight Mismatch: {MeldRequest.RingRight.ItemName} - {equipSlots[EquipmentSlot.Ring2]?.Name}");
            }

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Item Mismatch!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AmountEstimate_Click(object sender, EventArgs e)
        {
            // TODO: Add estimated amounts of materia needed.
            MessageBox.Show("WIP", "Not Ready Yet");
        }
    }
}