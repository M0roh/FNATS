using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace VoidspireStudio.FNATS.Sounds
{ 
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public enum AudioType
        {
            Default,
            Music,
            SFX,
            Ambient
        }

        public static AudioManager Instance { get; private set; }

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerGroup _ambientGroup;
        [SerializeField] private AudioMixerGroup _sfxGroup;

        [Header("Sounds Sources")]
        [SerializeField] private AudioSource _2dSource;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private GameObject _emptyVolumeObject;

        private readonly Dictionary<AudioSource, bool> _audioSources = new();

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
                if (_audioSources[source])
                {
                    Destroy(source.gameObject);
                    continue;
                }

                if (source == null || !source.isPlaying)
                {
                    _audioSources.Remove(source);
                    continue;
                }

                if (Time.timeScale == 0f)
                    source.Pause();
                else
                    source.UnPause();
            }

            if (Time.timeScale == 0f)
                _audioSources
                    .Select(audio => audio.Key)
                    .Where(source => source.outputAudioMixerGroup == _musicGroup && source.isPlaying)
                    .ForEach(source => source.Pause());
            else
                _audioSources
                    .Select(audio => audio.Key)
                    .Where(source => source.outputAudioMixerGroup == _musicGroup && !source.isPlaying)
                    .ForEach(source => source.UnPause());
        }

        public void UpdateSettings(Saves.AudioSettings audioSettings)
        {
            SetVolume(audioSettings.volumeMusic, AudioType.Music);
            SetVolume(audioSettings.ambientVolume, AudioType.Ambient);
            SetVolume(audioSettings.volumeSFX, AudioType.SFX);
        }

        public void SetVolume(float volume01, AudioType type)
        {
            if (type == AudioType.Default) return;

            float volumeDB = Mathf.Log10(Mathf.Max(volume01, 0.0001f)) * 20f;

            _mixer.SetFloat(type.ToString() + "Volume", volumeDB);
        }

        public void PlaySound2D(AudioClip clip, AudioType type = AudioType.Default)
        {
            _2dSource.PlayOneShot(clip);

            if (type != AudioType.Default)
                _2dSource.outputAudioMixerGroup = GetGroup(type);

            _audioSources[_2dSource] = false;
        }

        public void PlaySource(AudioSource source, AudioType type = AudioType.Default)
        {
            _audioSources[source] = false;

            if (type != AudioType.Default)
                _2dSource.outputAudioMixerGroup = GetGroup(type);

            source.pitch = Random.Range(0.95f, 1.05f);
            source.Play();
        }

        public void PlayLoopMusic(AudioClip clip)
        {
            _musicSource.Stop();
            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.Play();
        }

        public void StopLoopMusic() => _musicSource.Stop();
        public void PauseLoopMusic() => _musicSource.Pause();
        public void ResumeLoopMusic() => _musicSource.Play();

        public void PlaySound(AudioSource source, AudioClip clip, AudioType type = AudioType.Default)
        {
            source.PlayOneShot(clip);

            if (type != AudioType.Default)
                _2dSource.outputAudioMixerGroup = GetGroup(type);

            _audioSources[source] = false;
        }

        public void PlaySound(Vector3 position, AudioClip clip, AudioType type)
        {
            var obj = Instantiate(_emptyVolumeObject, position, Quaternion.identity);
            var source = obj.GetComponent<AudioSource>();

            source.outputAudioMixerGroup = GetGroup(type);

            source.PlayOneShot(clip);
            _audioSources[source] = true;
        }

        private AudioMixerGroup GetGroup(AudioType type)
        {
            return type switch
            {
                AudioType.Music => _musicGroup,
                AudioType.Ambient => _ambientGroup,
                AudioType.SFX => _sfxGroup,
                _ => _sfxGroup
            };
        }
    }
}