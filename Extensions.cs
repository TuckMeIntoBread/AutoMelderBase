using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using ff14bot.Enums;
using ff14bot.Helpers;
using LlamaLibrary.JsonObjects;
using LlamaLibrary.Logging;

namespace AutoMelder
{
    public static class Extensions
    {
        public static IEnumerable<Control> GetAllControls(this Control root)
        {
            var stack = new Stack<Control>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                Control next = stack.Pop();
                foreach (Control child in next.Controls)
                {
                    stack.Push(child);
                }

                yield return next;
            }
        }
        
        public static string AriyalaKey(this EquipmentSlot equipmentSlot)
        {
            switch (equipmentSlot)
            {
                case EquipmentSlot.MainHand:
                    return "mainhand";
                case EquipmentSlot.OffHand:
                    return "offhand";
                case EquipmentSlot.Head:
                    return "head";
                case EquipmentSlot.Body:
                    return "chest";
                case EquipmentSlot.Hands:
                    return "hands";
                case EquipmentSlot.Legs:
                    return "legs";
                case EquipmentSlot.Feet:
                    return "feet";
                case EquipmentSlot.Earring:
                    return "ears";
                case EquipmentSlot.Necklace:
                    return "neck";
                case EquipmentSlot.Bracelet:
                    return "wrist";
                case EquipmentSlot.Ring1:
                    return "ringLeft";
                case EquipmentSlot.Ring2:
                    return "ringRight";
                default:
                    throw new ArgumentOutOfRangeException(nameof(equipmentSlot), equipmentSlot, $"Unknown EquipmentSlot Type: {equipmentSlot}");
            }
        }

        public static string AriyalaKey(this MeldInfo info) => AriyalaKey(info.EquipType);

        public static string EtroKey(this EquipmentSlot equipmentSlot)
        {
            switch (equipmentSlot)
            {
                case EquipmentSlot.MainHand:
                    return "weapon";
                case EquipmentSlot.OffHand:
                    return "offHand";
                case EquipmentSlot.Head:
                    return "head";
                case EquipmentSlot.Body:
                    return "body";
                case EquipmentSlot.Hands:
                    return "hands";
                case EquipmentSlot.Legs:
                    return "legs";
                case EquipmentSlot.Feet:
                    return "feet";
                case EquipmentSlot.Earring:
                    return "ears";
                case EquipmentSlot.Necklace:
                    return "neck";
                case EquipmentSlot.Bracelet:
                    return "wrists";
                case EquipmentSlot.Ring1:
                    return "fingerL";
                case EquipmentSlot.Ring2:
                    return "fingerR";
                default:
                    throw new ArgumentOutOfRangeException(nameof(equipmentSlot), equipmentSlot, $"Unknown EquipmentSlot Type: {equipmentSlot}");
            }
        }

        public static string EtroKey(this MeldInfo info) => EtroKey(info.EquipType);

        private static readonly Regex CondensedNameRegex = new Regex(@"^(?:(?:Craftsman's|Gatherer's) )?(?<name>(?:[A-Za-z']+ ?){1,2}) Materia (?<rNumTier>[IVXLCDM]+)$");

        public static string ToFullString(this MateriaItem materiaItem)
        {
            string itemName = materiaItem.ItemName;
            Match match = CondensedNameRegex.Match(itemName);
            if (match.Success)
            {
                return $"{match.Groups["name"].Value} {match.Groups["rNumTier"].Value} {materiaItem.Stat} +{materiaItem.Value}";
            }
            return $"{materiaItem.ItemName} {materiaItem.Stat} +{materiaItem.Value}";
        }

        public static string ToShortJobString(this string jobString)
        {
            switch (jobString.ToLower())
            {
                case "carpenter":
                    return "CRP";
                case "blacksmith":
                    return "BLM";
                case "armorer":
                    return "ARM";
                case "goldsmith":
                    return "GSM";
                case "leatherworker":
                    return "LTW";
                case "weaver":
                    return "WVR";
                case "alchemist":
                    return "ALC";
                case "culinarian":
                    return "CUL";
                case "miner":
                    return "MIN";
                case "botanist":
                    return "BTN";
                case "fisher":
                    return "FSH";
                case "paladin":
                    return "PLD";
                case "warrior":
                    return "WAR";
                case "darkknight":
                case "dark knight":
                    return "DRK";
                case "gunbreaker":
                    return "GNB";
                case "monk":
                    return "MNK";
                case "dragoon":
                    return "DRG";
                case "ninja":
                    return "NIN";
                case "samurai":
                    return "SAM";
                case "reaper":
                    return "RPR";
                case "whitemage":
                case "white mage":
                    return "WHM";
                case "astrologian":
                    return "AST";
                case "scholar":
                    return "SCH";
                case "sage":
                    return "SGE";
                case "bard":
                    return "BRD";
                case "machinist":
                    return "MCH";
                case "dancer":
                    return "DNC";
                case "blackmage":
                case "black mage":
                    return "BLM";
                case "summoner":
                    return "SMN";
                case "redmage":
                case "red mage":
                    return "RDM";
                case "bluemage":
                case "blue mage":
                    return "BLU";
                default:
                    return jobString;
            }
        }

        public static float GetOvermeldChance(this MateriaItem item, MeldInfo info)
        {
            int meldSlot = info.GetIndexBySlot(item);
            byte guaranteedSlots = info.EquipSlot.Item.MateriaSlots;
            int overmeldSlot = meldSlot + 1 - guaranteedSlots;
            if (overmeldSlot <= 0) return 100f;
            return ResourceManager.OvermeldChances.Value[overmeldSlot][item.Tier];
        }

        public static float GetOvermeldChance(this MeldInfo info, int materiaSlot)
        {
            byte guaranteedSlots = info.EquipSlot.Item.MateriaSlots;
            var chanceDic = ResourceManager.OvermeldChances.Value;
            int materiaTier = info.GetSlotByIndex(materiaSlot).Tier;
            int overmeldIndex = materiaSlot - guaranteedSlots;
            try
            {
                return chanceDic[materiaTier][overmeldIndex];
            }
            catch (KeyNotFoundException e)
            {
                Log.Error($"MateriaTier {materiaTier} or OvermeldIndex {overmeldIndex} not found in dictionary. GuaranteedSlots: {guaranteedSlots}, Item: {info.EquipSlot.Item.EnglishName}{Environment.NewLine}{e}");
                throw;
            }
        }

        private static readonly LLogger Log = new LLogger("AutoMelder", Colors.Brown);
    }
}