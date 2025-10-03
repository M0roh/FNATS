using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.Sounds;

namespace VoidspireStudio.FNATS.PowerSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class Flashlight : MonoBehaviour, IElectricDevice
    {
        public static Flashlight Instance { get; private set; }

        [Header("Audio")]
        [SerializeField] private AudioClip _onSound;
        [SerializeField] private AudioClip _offSound;
        private AudioSource _audioSource;

        [Header("Light")]
        [SerializeField] private Light _flashlightLight;
        [SerializeField] private float _lowEnergyLightIntensivity;
        [SerializeField] private float _normalLightIntensivity;

        [Header("UI")]
        [SerializeField] private CanvasGroup _flahlightUI;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private float _fadeDelay = 0.7f;
        [SerializeField] private float _fadeDuration = 3f;

        private float _maxPower = 100f;
        private float _currentPower;

        private Coroutine _flahlightUIFade;

        public bool IsActive { get; private set; }
        public float Power => _currentPower;

        public float GetCurrentConsumption => 2 * NightManager.Instance.CurrentNight;

        private void Awake()
        {
            Instance = this;

            _currentPower = _maxPower;
            _audioSource = GetComponent<AudioSource>();
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
            _indicatorImage.fillAmount = Mathf.Clamp01(_currentPower / 100f);

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
            AudioManager.Instance.PlaySound(_audioSource, _offSound);

            IsActive = false;
            _flashlightLight.enabled = false;

            _flahlightUIFade = StartCoroutine(UIFade());
        }

        public void TurnOn()
        {
            AudioManager.Instance.PlaySound(_audioSource, _onSound);

            if (_currentPower <= 0f) return;

            IsActive = true;
            _flashlightLight.enabled = true;
            LightIntesivityUpdate();

            if (_flahlightUIFade != null)
                StopCoroutine(_flahlightUIFade);
            _flahlightUI.alpha = 1f;
        }

        private IEnumerator UIFade()
        {
            yield return new WaitForSeconds(_fadeDelay);

            var timer = _fadeDuration;
            while (_flahlightUI.alpha > 0)
            {
                _flahlightUI.alpha = Mathf.Lerp(_flahlightUI.alpha, 0f, Time.deltaTime / timer);
                timer -= Time.deltaTime;
                yield return null;
            }
        }

        public void Charge(float chargePower)
        {
            _currentPower += chargePower;

            if (_currentPower > _maxPower)
                _currentPower = _maxPower;
        }
    }
}
