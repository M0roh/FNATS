using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

namespace VoidspireStudio.FNATS.UI
{
    public class PowerUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private TMP_Text _powerText;

        [Header("Colors")]
        [SerializeField] private Color _fullPower;
        [SerializeField] private Color _halfPower;
        [SerializeField] private Color _zeroPower;

        private void OnEnable()
        {
            PowerSystem.PowerSystem.Instance.OnPowerChanged += PowerManager_OnPowerChanged;
        }

        private void OnDisable()
        {
            PowerSystem.PowerSystem.Instance.OnPowerChanged -= PowerManager_OnPowerChanged;
        }

        private void PowerManager_OnPowerChanged(float power)
        {
            var fill = Mathf.Clamp01(power / 100f);

            if (_indicatorImage != null)
                _indicatorImage.fillAmount = fill;

            if (_powerText != null)
                _powerText.text = $"{power:F1}%";

            if (fill > 0.5f)
            {
                float t = (fill - 0.5f) / 0.5f;
                _indicatorImage.color = Color.Lerp(_halfPower, _fullPower, t);
            }
            else
            {
                float t = fill / 0.5f;
                _indicatorImage.color = Color.Lerp(_zeroPower, _halfPower, t);
            }
        }
    }
}