using UnityEngine;
using VoidspireStudio.FNATS.Cameras;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.PowerSystem;

namespace VoidspireStudio.FNATS.Interactables
{
    public class PC : MonoBehaviour, IInteractable, IElectricDevice
    {
        public static PC Instance { get; private set; }

        [SerializeField] private Monitor _monitor;

        public bool IsActive { get; private set; } = false;

        public float GetCurrentConsumption => 0.04f * NightManager.Instance.CurrentNight;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            TurnOff();
        }

        public void OnInteract()
        {
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
        }

        public void TurnOn()
        {
            IsActive = true;
            _monitor.TurnOn();
        }
    }
}
