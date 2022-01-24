using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AutoMelder.MeldingLogic;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.Extensions;
using LlamaLibrary.JsonObjects;

namespace AutoMelder
{
    public partial class AutoMelderSettings : Form
    {
        public AutoMelderSettings()
        {
            InitializeComponent();
        }

        public MeldRequest MeldRequest;

        public bool IgnoreMismatched => ignoreMismatchedCBox.Checked;

        private void ImportButton_Click(object sender, EventArgs e)
        {
            MeldRequest = MeldLinkParser.ParseLinkOrId(importCodeBox.Text);
            MeldRequest.SetAllTextBoxes(this);
            importCodeBox.Text = string.Empty;
            Refresh();
            CheckItemMismatch();
            CheckMateriaMismatch();
        }

        private void CheckItemMismatch()
        {
            StringBuilder sb = new StringBuilder();
            Bag equipSlots = InventoryManager.GetBagByInventoryBagId(InventoryBagId.EquippedItems);
            foreach (MeldInfo meldInfo in MeldRequest.AllMelds())
            {
                if (meldInfo.ItemId > 0 && meldInfo.ItemId != (equipSlots[meldInfo.EquipType]?.RawItemId ?? 0))
                {
                    sb.AppendLine($"{meldInfo.EquipType.ToString()} Mismatch!");
                    sb.AppendLine($"{meldInfo.ItemName} - {equipSlots[meldInfo.EquipType].Name}");
                    sb.Append(Environment.NewLine);
                }
            }
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Item Mismatch!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CheckMateriaMismatch()
        {
            StringBuilder sb = new StringBuilder();
            Bag equipSlots = InventoryManager.GetBagByInventoryBagId(InventoryBagId.EquippedItems);
            foreach (MeldInfo meldInfo in MeldRequest.AllMelds())
            {
                if (meldInfo.ItemId == 0) continue;
                BagSlot equippedItem = equipSlots[meldInfo.EquipType];
                if (equippedItem == null || !equippedItem.IsValid || !equippedItem.IsFilled) continue;
                var meldedMateria = equippedItem.Materia();
                if (meldedMateria.Count == 0) continue;
                bool appendedItem = false;
                for (int i = 0; i < meldedMateria.Count; i++)
                {
                    MateriaItem currentMateria = meldedMateria[i];
                    MateriaItem desiredMateria = meldInfo.GetSlotByIndex(i);
                    if (desiredMateria == null || currentMateria.Key == desiredMateria.Key) continue;
                    if (!appendedItem)
                    {
                        sb.AppendLine($"{equippedItem.Name} Materia Mismatch!");
                        appendedItem = true;
                    }

                    sb.AppendLine($"#{i+1}: {desiredMateria.ToFullString()} - {currentMateria.ToFullString()}");
                }

                if (appendedItem) sb.Append(Environment.NewLine);
            }
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Materia Mismatch!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AmountEstimate_Click(object sender, EventArgs e)
        {
            var materiaCounts = new Dictionary<MateriaItem, int>();
            void IncreaseCount(MateriaItem materia, int count)
            {
                if (materiaCounts.ContainsKey(materia))
                {
                    materiaCounts[materia] += count;
                }
                else
                {
                    materiaCounts[materia] = count;
                }
            }
            foreach (MeldInfo meldInfo in MeldRequest.AllEnabledMelds(this))
            {
                if (meldInfo.ItemId == 0) continue;
                int currentCount = meldInfo.EquipSlot.MateriaCount();
                for (int i = currentCount; i < 5; i++)
                {
                    MateriaItem desiredMateria = meldInfo.GetSlotByIndex(i);
                    if (desiredMateria == null) break;
                    var guaranteedSlots = meldInfo.EquipSlot.Item.MateriaSlots;
                    if (i < guaranteedSlots)
                    {
                        IncreaseCount(desiredMateria, 1);
                    }
                    else
                    {
                        float meldChance = meldInfo.GetOvermeldChance(i);
                        if (meldChance == 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(meldChance), meldChance, "Trying to calculate meld chance for a materia that can't be overmelded into that slot!");
                        }
                        var materiaNeeded = (int)Math.Ceiling(100 / meldChance);
                        IncreaseCount(desiredMateria, materiaNeeded);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (var materiaCount in materiaCounts)
            {
                sb.AppendLine($"{materiaCount.Key.ToFullString()} x{materiaCount.Value}");
            }

            sb.AppendLine("Only accounts for current MH/OH!");
            MessageBox.Show(sb.ToString(), "Estimated Materia Needed", MessageBoxButtons.OK);
        }
    }
}