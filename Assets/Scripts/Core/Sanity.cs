using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using VoidspireStudio.FNATS.Nights;

namespace VoidspireStudio.FNATS.Core
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

        void Start()
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
            float lightLevel = GetLightIntensity();

            if (lightLevel < _darkLightLevel || Player.Instance.PlayerFlashlight.IsActive)
                _sanity -= _sanityDrain;
            else
                _sanity += _sanityRegeneration;

            EffectsUpdate(lightLevel);

            if (_sanity <= 0f)
                Debug.Log("Вы умерли от рассудка.");
        }

        private float GetLightIntensity()
        {
            LightProbes.GetInterpolatedProbe(Player.Instance.HeadPosition, null, out SphericalHarmonicsL2 probe);
            if (probe == null)
                return (probe[0, 0] + probe[1, 0] + probe[2, 0]) / 3f;
            else
                return 0f;
        }

        private void EffectsUpdate(float lightLevel)
        {
            _colorAdjust.saturation.value = Mathf.Lerp(-100f, 0f, lightLevel);
            _colorAdjust.postExposure.value = Mathf.Lerp(-2f, 0f, lightLevel);
            _vignette.intensity.value = 1f - lightLevel;
        }
    }
}