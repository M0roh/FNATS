using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Saves;
using VoidspireStudio.FNATS.Sounds;

namespace VoidspireStudio.FNATS.UI.Menus.SettingsTabs
{
    public class AudioSettings : MonoBehaviour
    {
        [Header("Music Volume")]
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private TMP_Text _musicVolumeDisplay;

        [Header("Ambient Volume")]
        [SerializeField] private Slider _ambientVolumeSlider;
        [SerializeField] private TMP_Text _ambientVolumeDisplay;

        [Header("SFX Volume")]
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private TMP_Text _sfxVolumeDisplay;

        private void Start()
        {
            var audioSettings = SaveManager.LastSavedData.audio;

            _musicVolumeDisplay.text = $"{audioSettings.volumeMusic * 100f}%";
            _musicVolumeSlider.value = audioSettings.volumeMusic;

            _ambientVolumeDisplay.text = $"{audioSettings.ambientVolume * 100f}%";
            _ambientVolumeSlider.value = audioSettings.ambientVolume;

            _sfxVolumeDisplay.text = $"{audioSettings.volumeSFX * 100f}%";
            _sfxVolumeSlider.value = audioSettings.volumeSFX;
        }

        private void OnEnable()
        {
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChange);
            _ambientVolumeSlider.onValueChanged.AddListener(OnAmbientVolumeChange);
            _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChange);         
        }

        private void OnDisable()
        {
            _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChange);
            _ambientVolumeSlider.onValueChanged.RemoveListener(OnAmbientVolumeChange);
            _sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChange);          
        }

        private void OnMusicVolumeChange(float volume)
        {
            volume = Mathf.Round(volume * 100) / 100;
            _musicVolumeDisplay.text = $"{volume * 100}%";

            SaveManager.LastSavedData.audio.volumeMusic = volume;
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioType.Music);
        }

        private void OnAmbientVolumeChange(float volume)
        {
            volume = Mathf.Round(volume * 100) / 100;
            _ambientVolumeDisplay.text = $"{volume * 100}%";

            SaveManager.LastSavedData.audio.ambientVolume = volume;
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioType.Ambient);
        }

        private void OnSFXVolumeChange(float volume)
        {
            volume = Mathf.Round(volume * 100) / 100;
            _sfxVolumeDisplay.text = $"{volume * 100}%";

            SaveManager.LastSavedData.audio.volumeSFX = volume;
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioType.SFX);
        }   
    }
}