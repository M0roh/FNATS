using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.Player;
using VoidspireStudio.FNATS.PowerSystem;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Interactables
{
    public class FlashlightCharger : MonoBehaviour, IInteractable, IElectricDevice
    {
        [Header("Settings")]
        [SerializeField] private GameObject _flashlightInChargeObject;
        [SerializeField] private float _chargeInTick = 5f;
        [SerializeField] private LocalizedString _interactTip;

        [Header("Indicator")]
        [SerializeField] private Light _chargeIndicator;
        [SerializeField] private Material _indicatorMaterial;

        [Header("Colors")]
        [SerializeField] private Color _fullEnergyColor;
        [SerializeField] private Color _halfEnergy;
        [SerializeField] private Color _lowEnergy;
        [SerializeField] private Color _veryLowEnergy;

        public bool IsActive { get; private set; }

        public LocalizedString InteractTip => _interactTip;

        public bool CanInteract => true;

        public float GetCurrentConsumption => 0.08f * NightManager.Instance.CurrentNight;

        private void Start()
        {
            this.RegisterDevice();
            TurnOff();
        }

        private void OnDestroy()
        {
            this.UnregisterDevice();
        }

        private void OnEnable()
        {
            NightTime.OnTick += Charge;
        }

        private void Charge(GameTime _)
        {
            if (!IsActive) return;

            Player.Player.Instance.PlayerFlashlight.Charge(_chargeInTick);
            IndicatorUpdate();
        }

        public void IndicatorUpdate()
        {
            float charge = Player.Player.Instance.PlayerFlashlight.Power;
            if (charge > 99f)
                _chargeIndicator.color = _fullEnergyColor;
            else if (charge > 50f)
                _chargeIndicator.color = _halfEnergy;
            else if (charge > 25f)
                _chargeIndicator.color = _lowEnergy;
            else
                _chargeIndicator.color = _veryLowEnergy;

            _indicatorMaterial.color = _chargeIndicator.color;
        }

        public void OnInteract()
        {
            if (PowerSystem.PowerSystem.Instance.IsStopped)
            {
                if (!IsActive)
                {
                    Player.Player.Instance.FlashlightPickup();
                    _flashlightInChargeObject.SetActive(false);
                }
				return;
            }

            if (IsActive)
            {
                Player.Player.Instance.FlashlightPickup();
                _flashlightInChargeObject.SetActive(false);
                TurnOff();
            }
            else
            {
                Player.Player.Instance.FlashlightDrop();
                _flashlightInChargeObject.SetActive(true);
                TurnOn();
            }
        }

        public void OnInteractEnd() { }

        public void TurnOff()
        {
            IsActive = false;
            _chargeIndicator.enabled = false;
        }

        public void TurnOn()
        {
            if (PowerSystem.PowerSystem.Instance.IsStopped) return;

            IsActive = true;
            _chargeIndicator.enabled = true;
            IndicatorUpdate();
        }
    }
}
