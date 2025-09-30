using UnityEngine;
using VoidspireStudio.FNATS.Nights;

namespace VoidspireStudio.FNATS.Sounds.Game
{
    [RequireComponent(typeof(AudioSource))]
    public class ClockSounds : MonoBehaviour
    {
        [SerializeField] private AudioClip _onFullHourSound;
        [SerializeField] private AudioClip _onTickSound;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void OnEnable()
        {
            NightTime.OnTick += NightTime_OnTick;
        }

        private void OnDisable()
        {
            NightTime.OnTick -= NightTime_OnTick;
        }

        private void NightTime_OnTick(GameTime time)
        {
            if (time.Minute == 0)
                AudioManager.Instance.PlaySound(_audioSource, _onFullHourSound, AudioManager.AudioType.SFX);
            else
                AudioManager.Instance.PlaySound(_audioSource, _onTickSound, AudioManager.AudioType.SFX);
        }
    }
}