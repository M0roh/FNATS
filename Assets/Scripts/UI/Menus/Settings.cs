using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus
{
    public class Settings : MonoBehaviour
    {
        [Header("Music Volume")]
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private TMP_Text _musicVolumeDisplay;

        [Header("SFX Volume")]
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private TMP_Text _sfxVolumeDisplay;

        [Header("Mouse Sensitivity")]
        [SerializeField] private Slider _mouseSensitivitySlider;
        [SerializeField] private TMP_Text _mouseSensitivityDisplay;

        [Header("Language")]
        [SerializeField] private TMP_Dropdown _languageDropdown;

        public void Start()
        {
            _languageDropdown.ClearOptions();
            _languageDropdown.AddOptions(LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.LocaleName).ToList());

            _musicVolumeDisplay.text = $"{SaveManager.LastSavedData.volumeMusic:F2}";
            _musicVolumeSlider.value = SaveManager.LastSavedData.volumeMusic;

            _sfxVolumeDisplay.text = $"{SaveManager.LastSavedData.volumeSFX:F2}";
            _sfxVolumeSlider.value = SaveManager.LastSavedData.volumeSFX;

            _mouseSensitivityDisplay.text = $"{SaveManager.LastSavedData.mouseSensitivity:F2}";
            _mouseSensitivitySlider.value = SaveManager.LastSavedData.mouseSensitivity;

            _languageDropdown.value = SaveManager.LastSavedData.languageIndex;
        }

        public void OnMusicVolumeChange(float volume)
        {
            volume = Mathf.Round(volume * 100) / 100;
            _musicVolumeDisplay.text = $"{volume}%";

            SaveManager.LastSavedData.volumeMusic = volume;
            AudioManager.Instance.UpdateMusicVolume(volume);
        }

        public void OnSFXVolumeChange(float volume)
        {
            volume = Mathf.Round(volume * 100) / 100;
            _sfxVolumeDisplay.text = $"{volume}%";

            SaveManager.LastSavedData.volumeSFX = volume;
            AudioManager.Instance.UpdateSFXVolume(volume);
        }

        public void OnMouseSensitivityChange(float sensitivity)
        {
            _sfxVolumeDisplay.text = $"{sensitivity}";
            SaveManager.LastSavedData.mouseSensitivity = sensitivity;
        }

        public void OnLanguageChanged(int localeIndex)
        {
            _languageDropdown.interactable = false;

            var selectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

            LocalizationSettings.SelectedLocale = selectedLocale;
            SaveManager.LastSavedData.languageIndex = localeIndex;
        }

        private void OnLocaleChanged(Locale _)
        {
            _languageDropdown.interactable = true;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        public void BackButton()
        {
            SaveManager.SaveGame();
            UIManager.Instance.BackToMenu(gameObject);
        }
    }
}
