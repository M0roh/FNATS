using UnityEngine;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private void Awake() => Instance = this;

        public void UpdateSettings(Saves.AudioSettings audioSettings)
        {
            SetMusicVolume(audioSettings.volumeMusic);
            SetAmbientVolume(audioSettings.ambientVolume);
            SetSFXVolume(audioSettings.volumeSFX);
        }

        public void SetMusicVolume(float volume) { }
        public void SetAmbientVolume(float volume) { }
        public void SetSFXVolume(float volume) { }
    }
}