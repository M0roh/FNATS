using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.PowerSystem;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Interactables
{
    public class LightSwitcher : MonoBehaviour, IInteractable, IElectricDevice
    {
        [SerializeField] private LocalizedString _interactTip;
        [SerializeField] private GameObject _light;
        [SerializeField] private GameObject _switchObject;
        [SerializeField] private Quaternion _onRotation;
        [SerializeField] private Quaternion _offRotation;

        public bool IsActive { get; private set; } = false;

        public LocalizedString InteractTip => _interactTip;

        public bool CanInteract => true;

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
            if (!IsActive || PowerSystem.PowerSystem.Instance.IsStopped)
            {
                _light.SetActive(false);
                _switchObject.transform.localRotation = _offRotation;
            }
            else if (!PowerSystem.PowerSystem.Instance.IsStopped)
            {
                _light.SetActive(true);
                _switchObject.transform.localRotation = _onRotation;
            }
        }
    }
}