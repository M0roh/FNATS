using System;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Cameras;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.PowerSystem;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Interactables
{
    public class PC : MonoBehaviour, IInteractable, IElectricDevice, IMachineEvents
    {
        public static PC Instance { get; private set; }

        [SerializeField] private Monitor _monitor;
        [SerializeField] private LocalizedString _interactTip;

        public bool IsActive { get; private set; } = false;

        public LocalizedString InteractTip => _interactTip;

        public float GetCurrentConsumption => 0.04f * NightManager.Instance.CurrentNight;

        public bool CanInteract => true;

        public event Action<bool> OnActiveChange;
        public event Action OnBroken;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            this.RegisterDevice();
            TurnOff();
        }

        private void OnDestroy()
        {
            this.UnregisterDevice();
        }

        public void OnInteract()
        {
            if (PowerSystem.PowerSystem.Instance.IsStopped) return;

            if (IsActive)
                TurnOff();
            else
                TurnOn();
        }

        public void OnInteractEnd() { }

        public void TurnOff()
        {
            IsActive = false;
            _monitor.TurnOff();
            SecurityCamerasManager.Instance.CloseCameras(new());
            OnActiveChange?.Invoke(false);
        }

        public void TurnOn()
        {
            if (PowerSystem.PowerSystem.Instance.IsStopped) return;

            IsActive = true;
            _monitor.TurnOn();
            OnActiveChange?.Invoke(true);
        }
    }
}
