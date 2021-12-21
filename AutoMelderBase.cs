using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Managers;
using LlamaLibrary.Extensions;
using LlamaLibrary.JsonObjects;
using LlamaLibrary.Logging;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace AutoMelder
{
    public class AutoMelder : BotBase
    {
        private static readonly LLogger Log = new LLogger("AutoMelder", Colors.Teal);

        private Composite _root;
        public override string Name => "AutoMelder";
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public override Composite Root => _root;
        public override bool WantButton { get; } = true;
        private AutoMelderSettings _settings;
        private static bool _isDone = false;

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
            _isDone = false;
            _root = new ActionRunCoroutine(r => Run());
        }

        private async Task<bool> Run()
        {
            if (_isDone) return false;
            _lastValidItem = 0;
            _lastValidMateria = 0;
            if (_settings?.MeldRequest == null)
            {
                Log.Error("Nothing imported!");
                TreeRoot.Stop();
                return false;
            }
            MeldRequest meldRequest = _settings.MeldRequest;
            await MeldItem(meldRequest.Head);
            await MeldItem(meldRequest.Chest);
            await MeldItem(meldRequest.Hands);
            await MeldItem(meldRequest.Legs);
            await MeldItem(meldRequest.Feet);
            await MeldItem(meldRequest.Ears);
            await MeldItem(meldRequest.Neck);
            await MeldItem(meldRequest.Wrist);
            await MeldItem(meldRequest.RingLeft);
            await MeldItem(meldRequest.RingRight);
            await MeldItem(meldRequest.MainHand);
            await MeldItem(meldRequest.OffHand);

            _isDone = true;
            TreeRoot.Stop("Done melding materia.");
            return false;
        }

        private static async Task MeldItem(MeldInfo meldInfo)
        {
            BagSlot equipSlot = meldInfo?.EquipSlot;
            if (equipSlot == null || !equipSlot.IsValid || !equipSlot.IsFilled) return;
            int alreadyMeldedCount = equipSlot.MateriaCount();
            if (alreadyMeldedCount >= 5 || meldInfo.GetSlotByIndex(0) == null) return;
            Log.Debug($"Trying to affix materia to {equipSlot.Name}");
            for (int i = alreadyMeldedCount; i < 5; i++)
            {
                await Coroutine.Sleep(800);
                MateriaItem materiaToMeld = meldInfo.GetSlotByIndex(i);
                if (materiaToMeld == null) break;
                BagSlot materiaSlot = GetMateriaSlot(materiaToMeld);
                if (materiaSlot == null || !materiaSlot.IsValid || !materiaSlot.IsFilled)
                {
                    Log.Error($"We don't have any {materiaToMeld.ToFullString()} to meld {equipSlot.Name}'s Slot #{i+1} with!");
                    return;
                }

                if (!await TryAffixMateria(equipSlot, materiaSlot)) return;
                await Coroutine.Sleep(800);
                var alreadyMeldedInfo = equipSlot.Materia();
                if (alreadyMeldedInfo.Count <= i)
                {
                    Log.Error("Failed to meld materia! Maybe we're out?");
                    return;
                }

                if (alreadyMeldedInfo[i].Key != materiaToMeld.Key)
                {
                    Log.Error("Last materia melded doesn't match what we were supposed to meld! Oops...");
                    _isDone = true;
                    TreeRoot.Stop();
                    return;
                }
            }

            if (MateriaAttach.Instance.IsOpen)
            {
                Log.Debug("Closing meld window");
                MateriaAttach.Instance.Close();
                await Coroutine.Wait(7000, () => !MateriaAttach.Instance.IsOpen);
            }
        }

        private static async Task<bool> TryAffixMateria(BagSlot itemToAffix, BagSlot materiaToUse)
        {
            Log.Information($"Trying to affix {materiaToUse.Name} to {itemToAffix.Name}");
            if (!await OpenMeldWindow(itemToAffix))
            {
                Log.Error("Failed to open meld window!");
                TreeRoot.Stop("Materia Melding Failed");
                return false;
            }

            if (!await OpenMateriaAttachDialog())
            {
                Log.Error("Failed to open materia attach dialog!");
                TreeRoot.Stop("Materia Melding Failed");
                return false;
            }

            Log.Debug("Sending BagSlot Affix");
            await Coroutine.Wait(1500, () => AgentMeld.Instance.CanMeld);
            if (!materiaToUse.IsValid || !materiaToUse.IsFilled) return false;
            itemToAffix.AffixMateria(materiaToUse, true);
            await Coroutine.Wait(20000, () => !AgentMeld.Instance.Ready);
            await Coroutine.Wait(20000, () => AgentMeld.Instance.Ready);
            await Coroutine.Wait(7000, () => !MateriaAttachDialog.Instance.IsOpen);
            MateriaAttach.Instance.Close();
            await Coroutine.Wait(7000, () => !MateriaAttach.Instance.IsOpen);
            return true;
        }

        private static async Task<bool> OpenMeldWindow(BagSlot itemToAffix)
        {
            if (MateriaAttach.Instance.IsOpen) return true;
            for (int i = 0; i < 2; i++)
            {
                Log.Debug("Opening meld window");
                itemToAffix.OpenMeldInterface();
                await Coroutine.Wait(5000, () => MateriaAttach.Instance.IsOpen);
                if (MateriaAttach.Instance.IsOpen)
                {
                    return true;
                }
            }

            return false;
        }

        private static int _lastValidItem;
        private static int _lastValidMateria;

        private static async Task<bool> OpenMateriaAttachDialog()
        {
            if (MateriaAttachDialog.Instance.IsOpen) return true;
            Log.Debug("Opening materia attach dialog");
            // Try to select based on materia alone first... we should have them open due to meld requesting the specific item needed?
            for (int i = _lastValidMateria; i < 10; i++)
            {
                MateriaAttach.Instance.ClickMateria(i);
                int attachWait = _lastValidMateria > 0 && i == _lastValidMateria ? 1500 : 200;
                await Coroutine.Wait(attachWait, () => MateriaAttachDialog.Instance.IsOpen);
                if (MateriaAttachDialog.Instance.IsOpen)
                {
                    _lastValidMateria = i;
                    goto exitLoop;
                }
            }
            for (int i = _lastValidItem; i < 12; i++)
            {
                MateriaAttach.Instance.ClickItem(i);
                int clickWait = _lastValidItem > 0 && i == _lastValidItem ? 500 : 250;
                await Coroutine.Sleep(clickWait);
                for (int j = _lastValidMateria; j < 10; j++)
                {
                    MateriaAttach.Instance.ClickMateria(j);
                    int attachWait = _lastValidMateria > 0 && j == _lastValidMateria ? 1500 : 200;
                    await Coroutine.Wait(attachWait, () => MateriaAttachDialog.Instance.IsOpen);
                    if (MateriaAttachDialog.Instance.IsOpen)
                    {
                        _lastValidMateria = j;
                        _lastValidItem = i;
                        goto exitLoop;
                    }
                }
            }
            exitLoop:
            await Coroutine.Wait(3000, () => AgentMeld.Instance.CanMeld || AgentMeld.Instance.Ready);
            return MateriaAttachDialog.Instance.IsOpen;
        }

        private static BagSlot GetMateriaSlot(MateriaItem materiaItem)
        {
            return InventoryManager.FilledSlots.FirstOrDefault(x => x.RawItemId == materiaItem.Key);
        }

        public override void Stop()
        {
            _root = null;
        }
    }
}