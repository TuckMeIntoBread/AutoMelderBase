namespace AutoMelder.MeldingLogic
{
    public class MeldRequest
    {
        public MeldInfo MainHand { get; } = new MeldInfo();
        
        public MeldInfo OffHand { get; } = new MeldInfo();
        
        public MeldInfo Head { get; } = new MeldInfo();
        
        public MeldInfo Chest { get; } = new MeldInfo();
        
        public MeldInfo Hands { get; } = new MeldInfo();
        
        public MeldInfo Legs { get; } = new MeldInfo();
        
        public MeldInfo Feet { get; } = new MeldInfo();
        
        public MeldInfo Ears { get; } = new MeldInfo();
        
        public MeldInfo Neck { get; } = new MeldInfo();
        
        public MeldInfo Wrist { get; } = new MeldInfo();
        
        public MeldInfo RingLeft { get; } = new MeldInfo();
        
        public MeldInfo RingRight { get; } = new MeldInfo();
    }
}