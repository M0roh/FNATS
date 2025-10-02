using UnityEngine;
using VoidspireStudio.FNATS.Nights;

namespace VoidspireStudio.FNATS.Sounds.Game
{
    [RequireComponent(typeof(AudioSource))]
    public class ClockSounds : MonoBehaviour
    {
        [SerializeField] private AudioClip _onFullHourSound;
        [SerializeField] private AudioClip _tickSound;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _tickSound;
            _audioSource.loop = true;
        }

        public void OnEnable()
        {
            NightTime.OnTick += NightTime_OnTick;
            AudioManager.Instance.PlaySource(_audioSource, AudioManager.AudioType.Ambient);
        }

        private void OnDisable()
        {
            _audioSource.Stop();
            NightTime.OnTick -= NightTime_OnTick;
        }

        private void NightTime_OnTick(GameTime time)
        {
            if (time.Minute == 0)
                AudioManager.Instance.PlaySound(_audioSource, _onFullHourSound, AudioManager.AudioType.Ambient);
        }
    }
}