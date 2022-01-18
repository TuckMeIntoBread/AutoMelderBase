using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using ff14bot.Enums;

namespace AutoMelder.MeldingLogic
{
    public class MeldRequest
    {
        public MeldInfo MainHand { get; } = new MeldInfo(EquipmentSlot.MainHand);
        
        public MeldInfo OffHand { get; } = new MeldInfo(EquipmentSlot.OffHand);
        
        public MeldInfo Head { get; } = new MeldInfo(EquipmentSlot.Head);
        
        public MeldInfo Chest { get; } = new MeldInfo(EquipmentSlot.Body);
        
        public MeldInfo Hands { get; } = new MeldInfo(EquipmentSlot.Hands);
        
        public MeldInfo Legs { get; } = new MeldInfo(EquipmentSlot.Legs);
        
        public MeldInfo Feet { get; } = new MeldInfo(EquipmentSlot.Feet);
        
        public MeldInfo Ears { get; } = new MeldInfo(EquipmentSlot.Earring);
        
        public MeldInfo Neck { get; } = new MeldInfo(EquipmentSlot.Necklace);
        
        public MeldInfo Wrist { get; } = new MeldInfo(EquipmentSlot.Bracelet);
        
        public MeldInfo RingLeft { get; } = new MeldInfo(EquipmentSlot.Ring1);
        
        public MeldInfo RingRight { get; } = new MeldInfo(EquipmentSlot.Ring2);

        public void SetAllTextBoxes(Form settingsForm)
        {
            foreach (MeldInfo meldInfo in AllMelds())
            {
                meldInfo.SetTextBoxes(settingsForm);
            }
        }

        public IEnumerable<MeldInfo> AllMelds()
        {
            foreach (PropertyInfo propInfo in GetType().GetProperties())
            {
                if (propInfo.GetValue(this) is MeldInfo meldInfo)
                {
                    yield return meldInfo;
                }
            }
        }
        
        public IEnumerable<MeldInfo> AllEnabledMelds(AutoMelderSettings settingsForm)
        {
            foreach (PropertyInfo propInfo in GetType().GetProperties())
            {
                if (propInfo.GetValue(this) is MeldInfo meldInfo)
                {
                    if (meldInfo.IsValid && meldInfo.IsEnabled(settingsForm))
                    {
                        yield return meldInfo;
                    }
                }
            }
        }

        public static readonly MeldRequest Empty = new MeldRequest();
    }
}