using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace AutoMelder.MeldingLogic
{
    public class MeldRequest
    {
        public MeldInfo MainHand { get; } = new MeldInfo("mainhand");
        
        public MeldInfo OffHand { get; } = new MeldInfo("offhand");
        
        public MeldInfo Head { get; } = new MeldInfo("head");
        
        public MeldInfo Chest { get; } = new MeldInfo("chest");
        
        public MeldInfo Hands { get; } = new MeldInfo("hands");
        
        public MeldInfo Legs { get; } = new MeldInfo("legs");
        
        public MeldInfo Feet { get; } = new MeldInfo("feet");
        
        public MeldInfo Ears { get; } = new MeldInfo("ears");
        
        public MeldInfo Neck { get; } = new MeldInfo("neck");
        
        public MeldInfo Wrist { get; } = new MeldInfo("wrist");
        
        public MeldInfo RingLeft { get; } = new MeldInfo("ringLeft");
        
        public MeldInfo RingRight { get; } = new MeldInfo("ringRight");

        public void SetAllInfo(JToken info)
        {
            MainHand.SetInfo(info);
            OffHand.SetInfo(info);
            Head.SetInfo(info);
            Chest.SetInfo(info);
            Hands.SetInfo(info);
            Legs.SetInfo(info);
            Feet.SetInfo(info);
            Ears.SetInfo(info);
            Neck.SetInfo(info);
            Wrist.SetInfo(info);
            RingLeft.SetInfo(info);
            RingRight.SetInfo(info);
        }

        public void SetAllTextBoxes(Form settingsForm)
        {
            MainHand.SetTextBoxes(settingsForm);
            OffHand.SetTextBoxes(settingsForm);
            Head.SetTextBoxes(settingsForm);
            Chest.SetTextBoxes(settingsForm);
            Hands.SetTextBoxes(settingsForm);
            Legs.SetTextBoxes(settingsForm);
            Feet.SetTextBoxes(settingsForm);
            Ears.SetTextBoxes(settingsForm);
            Neck.SetTextBoxes(settingsForm);
            Wrist.SetTextBoxes(settingsForm);
            RingLeft.SetTextBoxes(settingsForm);
            RingRight.SetTextBoxes(settingsForm);
        }
    }
}