using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using AutoMelder.MeldingLogic;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
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
            StopMeldingSet.Clear();
            if (_settings?.MeldRequest == null)
            {
                Log.Error("Nothing imported!");
                _isDone = true;
                TreeRoot.Stop();
                return false;
            }
            MeldRequest meldRequest = _settings.MeldRequest;
            // Meld all guaranteed slots.
            foreach (MeldInfo meldInfo in meldRequest.AllEnabledMelds(_settings))
            {
                byte guaranteedSlots = meldInfo.EquipSlot.Item.MateriaSlots;
                for (int i = meldInfo.EquipSlot.MateriaCount(); i < guaranteedSlots; i++)
                {
                    await MeldItemBySlot(meldInfo, i);
                }
            }

            // Meld remaining slots in non-MH/OH.
            for (int i = 1; i < 5; i++)
            {
                foreach (MeldInfo meldInfo in meldRequest.AllEnabledMelds(_settings).Where(x => x.EquipType != EquipmentSlot.MainHand && x.EquipType != EquipmentSlot.OffHand))
                {
                    await MeldItemBySlot(meldInfo, i);
                }
            }
            
            // Meld MH/OH.
            for (int i = 1; i < 5; i++)
            {
                foreach (MeldInfo meldInfo in meldRequest.AllEnabledMelds(_settings).Where(x => x.EquipType == EquipmentSlot.MainHand || x.EquipType == EquipmentSlot.OffHand))
                {
                    await MeldItemBySlot(meldInfo, i);
                }
            }

            _isDone = true;
            TreeRoot.Stop("Done melding materia.");
            return false;
        }

        private static readonly HashSet<EquipmentSlot> StopMeldingSet = new HashSet<EquipmentSlot>();

        private static async Task MeldItemBySlot(MeldInfo meldInfo, int index)
        {
            if (index >= 5) return;
            MateriaItem materiaToMeld = meldInfo?.GetSlotByIndex(index);
            if (materiaToMeld == null || StopMeldingSet.Contains(meldInfo.EquipType)) return;
            BagSlot equipSlot = meldInfo.EquipSlot;
            if (equipSlot == null || !equipSlot.IsValid || !equipSlot.IsFilled) return;
            int alreadyMeldedCount = equipSlot.MateriaCount();
            if (alreadyMeldedCount > index) return;
            Log.Information($"Trying to affix materia to {equipSlot.Name}'s Slot #{index+1}");
            await Coroutine.Sleep(800);
            BagSlot materiaSlot = GetMateriaSlot(materiaToMeld);
            if (materiaSlot == null || !materiaSlot.IsValid || !materiaSlot.IsFilled)
            {
                Log.Error($"We don't have any {materiaToMeld.ToFullString()} to meld {equipSlot.Name}'s Slot #{index+1} with!");
                StopMeldingSet.Add(meldInfo.EquipType);
                return;
            }

            if (!await TryAffixMateria(equipSlot, materiaSlot)) return;
            await Coroutine.Sleep(800);
            var alreadyMeldedInfo = equipSlot.Materia();
            if (alreadyMeldedInfo.Count <= index)
            {
                if (!materiaSlot.IsValid || !materiaSlot.IsFilled || materiaSlot.Count == 0)
                {
                    Log.Warning($"Failed to meld! We've ran out of {materiaToMeld.ToFullString()}");
                    StopMeldingSet.Add(meldInfo.EquipType);
                    return;
                }

                Log.Error("Failed to meld materia! Unknown reason?");
                StopMeldingSet.Add(meldInfo.EquipType);
                return;
            }

            if (alreadyMeldedInfo[index].Key != materiaToMeld.Key)
            {
                Log.Error("Last materia melded doesn't match what we were supposed to meld! Oops...");
                _isDone = true;
                TreeRoot.Stop("Melded Wrong Materia");
                return;
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

        private static async Task<bool> OpenMateriaAttachDialog() => await MateriaAttach.Instance.OpenMateriaAttachDialog();

        private static BagSlot GetMateriaSlot(MateriaItem materiaItem)
        {
            return InventoryManager.FilledSlots.FirstOrDefault(x => x.RawItemId == materiaItem.Key);
        }

        public override void Stop()
        {
            _isDone = true;
            _root = null;
        }
    }
}