using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoidspireStudio.FNATS.Core
{ 
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public enum AudioType
        {
            Music,
            SFX,
            Ambient
        }

        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource _2dSource;
        [SerializeField] private GameObject _emptyVolumeObject;

        private readonly Dictionary<AudioType, float> _audioVolumes = new();
        private readonly Dictionary<AudioSource, (AudioType type, bool needDestroy)> _audioSources = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _2dSource = GetComponent<AudioSource>();
        }


        private void Update()
        {
            foreach (var source in _audioSources.Keys.ToList())
            {
                if (source == null || !source.isPlaying)
                {
                    _audioSources.Remove(source);
                }
            }
        }

        public void UpdateSettings(Saves.AudioSettings audioSettings)
        {
            SetVolume(audioSettings.volumeMusic, AudioType.Music);
            SetVolume(audioSettings.ambientVolume, AudioType.Ambient);
            SetVolume(audioSettings.volumeSFX, AudioType.SFX);
        }

        public void UpdateVolume()
        {
            foreach (var source in new Dictionary<AudioSource, (AudioType type, bool needDestroy)>(_audioSources))
            {
                source.Key.volume = _audioVolumes[source.Value.type];
            }
        }

        public void SetVolume(float volume, AudioType type)
        {
            _audioVolumes[type] = volume;
            UpdateVolume();
        }

        public void PlaySound2D(AudioClip clip, AudioType type)
        {
            _2dSource.PlayOneShot(clip);
            _2dSource.volume = _audioVolumes[type];
            _audioSources[_2dSource] = (type, false);
        }

        public void PlaySound(AudioSource source, AudioClip clip, AudioType type)
        {
            source.PlayOneShot(clip);
            source.volume = _audioVolumes[type];
            _audioSources[source] = (type, false);
        }

        public void PlaySound(Vector3 position, AudioClip clip, AudioType type)
        {
            var obj = Instantiate(_emptyVolumeObject, position, Quaternion.identity);
            var source = obj.GetComponent<AudioSource>();

            source.volume = _audioVolumes[type];
            source.PlayOneShot(clip);
            _audioSources[source] = (type, false);
        }
    }
}