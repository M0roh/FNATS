using UnityEngine;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.PowerSystem;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Interactables
{
    public class LightSwitcher : MonoBehaviour, IInteractable, IElectricDevice
    {
        [SerializeField] private Light _light;

        public bool IsActive { get; private set; } = false;

        public float GetCurrentConsumption => 0.01f * NightManager.Instance.CurrentNight;

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

            IsActive = !IsActive;
            LightUpdate();
        }

        public void OnInteractEnd() { }

        public void TurnOff()
        {
            IsActive = false;
            LightUpdate();
        }

        public void TurnOn()
        {
            if (PowerSystem.PowerSystem.Instance.IsStopped) return;

            IsActive = true;
            LightUpdate();
        }

        public void LightUpdate()
        {
            if (IsActive || PowerSystem.PowerSystem.Instance.IsStopped)
            {
                IsActive = false;
                _light.enabled = false;
            }
            else if (!PowerSystem.PowerSystem.Instance.IsStopped)
            {
                IsActive = true;
                _light.enabled = true;
            }
        }
    }
}