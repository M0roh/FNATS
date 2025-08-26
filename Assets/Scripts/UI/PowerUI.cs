using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoidspireStudio.FNATS.PowerSystem;

namespace VoidspireStudio.FNATS.UI
{
    public class PowerUI : MonoBehaviour
    {
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private TMP_Text _powerText;

        private void Awake()
        {
            _indicatorImage = GetComponent<Image>();
        }

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
            if (_indicatorImage != null)
                _indicatorImage.fillAmount = power / 100f;

            if (_powerText != null)
                _powerText.text = $"{power:F1}%";
        }
    }
}