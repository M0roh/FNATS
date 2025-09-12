using UnityEngine;

namespace VoidspireStudio.FNATS.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private void Awake() => Instance = this;

        public void UpdateMusicVolume(float volume) { }
        public void UpdateSFXVolume(float volume) { }
    }
}