using System;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot.AClasses;
using ff14bot.Behavior;
using LlamaLibrary.Logging;
using LlamaLibrary.Memory;
using TreeSharp;

namespace AutoMelder
{
    public class AutoMelder : BotBase
    {
        private static readonly LLogger Log = new LLogger("AutoMelder", Colors.Orange);

        private Composite _root;
        public override string Name => "AutoMelder";
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public override Composite Root => _root;
        public override bool WantButton { get; } = true;
        private AutoMelderSettings _settings;

        public override void Initialize()
        {
            OffsetManager.Init();
        }

        public override void OnButtonPress()
        {
            if (_settings == null || _settings.IsDisposed)
            {
                _settings = new AutoMelderSettings();
            }

            try
            {
                _settings.Show();
                _settings.Activate();
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public override void Start()
        {
            _root = new ActionRunCoroutine(r => Run());
        }

        private async Task<bool> Run()
        {
            return false;
        }

        public override void Stop()
        {
            _root = null;
        }
    }
}