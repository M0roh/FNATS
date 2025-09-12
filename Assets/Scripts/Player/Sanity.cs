using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using VoidspireStudio.FNATS.Nights;

namespace VoidspireStudio.FNATS.Player
{
    public class Sanity : MonoBehaviour
    {
        public static Sanity Instance { get; private set; }

        [SerializeField] private float _maxSanity = 100f;
        [SerializeField] private float _sanityDrain = 1f;
        [SerializeField] private float _sanityRegeneration = 5f;
        [SerializeField] private float _darkLightLevel = 0.15f;
        private float _sanity;

        [SerializeField] private Volume _volume;
        private Vignette _vignette;
        private ColorAdjustments _colorAdjust;

        private void Start()
        {
            _sanity = _maxSanity;

            if (_volume.profile.TryGet<Vignette>(out _vignette))
                _vignette.intensity.value = 0f;

            if (_volume.profile.TryGet<ColorAdjustments>(out _colorAdjust))
            {
                _colorAdjust.saturation.value = 0f;
                _colorAdjust.postExposure.value = 0f;
            }
        }

        private void OnEnable()
        {
            NightTime.OnTick += SanityDrain;
        }

        private void OnDisable()
        {
            NightTime.OnTick -= SanityDrain;
        }

        private void SanityDrain(GameTime _)
        {
            if (_sanity <= 0f) return;

            float lightLevel = GetLightLevel();

            if (lightLevel < _darkLightLevel)
                _sanity -= _sanityDrain;
            else if (_sanity < _maxSanity)
                _sanity += _sanityRegeneration;

            EffectsUpdate();

            if (_sanity <= 0f)
                Debug.Log("Вы умерли от рассудка.");
        }

        private float GetLightLevel()
        {
            float totalIntensity = 0f;
            foreach (Light light in FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (!light.enabled) continue;

                Vector3 dir = light.transform.position - Player.Instance.HeadPosition;
                float distance = dir.magnitude;

                if (Physics.Raycast(Player.Instance.HeadPosition, dir.normalized, distance))
                    continue;

                totalIntensity += light.intensity / (distance * distance);
            }

            return totalIntensity;
        }

        private void EffectsUpdate()
        {
            float sanityPercent = Mathf.Clamp01(_sanity / _maxSanity);

            _colorAdjust.saturation.value = Mathf.Lerp(-100f, 0f, sanityPercent);
            _colorAdjust.postExposure.value = Mathf.Lerp(-2f, 0f, sanityPercent);
            _vignette.intensity.value = Mathf.Lerp(1f, 0f, sanityPercent);
        }
    }
}