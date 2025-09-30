using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.Sounds;

namespace VoidspireStudio.FNATS.PowerSystem
{
    public class Flashlight : MonoBehaviour, IElectricDevice
    {
        public static Flashlight Instance { get; private set; }

        [Header("Взаимодействие")]
        [SerializeField] private LocalizedString _interactTip;
        [SerializeField] private AudioClip _interactSound;

        [Header("Свет")]
        [SerializeField] private Light _flashlightLight;
        [SerializeField] private float _lowEnergyLightIntensivity;
        [SerializeField] private float _normalLightIntensivity;

        private float _maxPower = 100f;
        private float _currentPower;

        public bool IsActive { get; private set; }
        public float Power => _currentPower;

        public LocalizedString InteractTip => _interactTip;

        public float GetCurrentConsumption => 2 * NightManager.Instance.CurrentNight;

        private void Awake()
        {
            Instance = this;

            _currentPower = _maxPower;
        }

        private void Start()
        {
            _flashlightLight.lightUnit = LightUnit.Lumen;
        }

        private void OnEnable()
        {
            NightTime.OnTick += PowerDrain;
            TurnOff();
        }

        private void OnDisable()
        {
            NightTime.OnTick -= PowerDrain;
        }

        private void PowerDrain(GameTime _)
        {
            if (!IsActive) return;

            if (_currentPower <= 0f)
            {
                TurnOff();
                return;
            }

            _currentPower -= GetCurrentConsumption;

            LightIntesivityUpdate();
        }

        public void LightIntesivityUpdate()
        {
            if (_currentPower <= _maxPower * 0.25f)
                _flashlightLight.intensity = _lowEnergyLightIntensivity;
            else
                _flashlightLight.intensity = _normalLightIntensivity;
        }

        public void TurnOff()
        {
            AudioManager.Instance.PlaySound2D(_interactSound, AudioManager.AudioType.SFX);

            IsActive = false;
            _flashlightLight.enabled = false;
        }

        public void TurnOn()
        {
            AudioManager.Instance.PlaySound2D(_interactSound, AudioManager.AudioType.SFX);

            if (_currentPower <= 0f) return;

            IsActive = true;
            _flashlightLight.enabled = true;
            LightIntesivityUpdate();
        }

        public void Charge(float chargePower)
        {
            _currentPower += chargePower;

            if (_currentPower > _maxPower)
                _currentPower = _maxPower;
        }
    }
}
