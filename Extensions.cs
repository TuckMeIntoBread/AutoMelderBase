using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AutoMelder.Ariyala;
using AutoMelder.MeldingLogic;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.JsonObjects;
using Newtonsoft.Json.Linq;

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

        public static bool IsTwoSlotGuaranteed(this BagSlot equipment)
        {
            return equipment.Item.MateriaSlots == 2;
        }

        public static bool IsTwoSlotGuaranteed(this EquipmentSlot equipmentSlot)
        {
            switch (equipmentSlot)
            {
                case EquipmentSlot.MainHand:
                case EquipmentSlot.OffHand:
                case EquipmentSlot.Earring:
                case EquipmentSlot.Necklace:
                case EquipmentSlot.Bracelet:
                case EquipmentSlot.Ring1:
                case EquipmentSlot.Ring2:
                    return false;
                case EquipmentSlot.Head:
                case EquipmentSlot.Body:
                case EquipmentSlot.Hands:
                case EquipmentSlot.Legs:
                case EquipmentSlot.Feet:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(equipmentSlot), equipmentSlot, $"Unknown EquipmentSlot Type: {equipmentSlot}");
            }
        }

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
    }
}