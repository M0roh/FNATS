using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus.SettingsTabs
{
    public class GameplaySettings : MonoBehaviour
    {
        [Header("Mouse Sensitivity")]
        [SerializeField] private Slider _mouseSensitivitySlider;
        [SerializeField] private TMP_Text _mouseSensitivityDisplay;

        [Header("Hints")]
        [SerializeField] private Toggle _hintsToggle;

        [Header("Language")]
        [SerializeField] private TMP_Dropdown _languageDropdown;

        public void Start()
        {
            _languageDropdown.ClearOptions();
            _languageDropdown.AddOptions(LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.LocaleName).ToList());

            var gameplaySettings = SaveManager.LastSavedData.gameplay;

            _mouseSensitivityDisplay.text = $"{gameplaySettings.mouseSensitivity:F2}";
            _mouseSensitivitySlider.value = gameplaySettings.mouseSensitivity;

            _hintsToggle.isOn = gameplaySettings.hints;

            _languageDropdown.value = gameplaySettings.languageIndex;
            _languageDropdown.interactable = true;
        }

        private void OnEnable()
        {
            _hintsToggle.onValueChanged.AddListener(OnHintsToggle);
            _mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChange);
            _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }

        private void OnDisable()
        {
            _hintsToggle.onValueChanged.RemoveListener(OnHintsToggle);
            _mouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSensitivityChange);
            _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
        }

        public void OnHintsToggle(bool hints)
        {
            SaveManager.LastSavedData.gameplay.hints = hints;
        }

        public void OnMouseSensitivityChange(float sensitivity)
        {
            _mouseSensitivityDisplay.text = $"{sensitivity}";
            SaveManager.LastSavedData.gameplay.mouseSensitivity = sensitivity;
        }

        public void OnLanguageChanged(int localeIndex)
        {
            _languageDropdown.interactable = false;

            var selectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

            LocalizationSettings.SelectedLocale = selectedLocale;
            SaveManager.LastSavedData.gameplay.languageIndex = localeIndex;
        }

        private void OnLocaleChanged(Locale _)
        {
            _languageDropdown.interactable = true;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }
    }
}