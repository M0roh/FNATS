using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.Sounds.Game
{
    [RequireComponent(typeof(AudioSource))]
    public class PCSounds : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip _onTurnOnSound;
        [SerializeField] private AudioClip _onTurnOffSound;
        [SerializeField] private AudioClip _onRunningSound;
        private AudioSource _audioSource;

        private Coroutine _soundsStarting;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            PC.Instance.OnActiveChange += OnPCActiveChange;
        }

        private void OnPCActiveChange(bool isActive)
        {
            if (isActive)
            {
                _soundsStarting = StartCoroutine(StartingSound());
            }
            else
            {
                if (_soundsStarting != null)
                    StopCoroutine(_soundsStarting);
                _audioSource.Stop();

                AudioManager.Instance.PlaySound(_audioSource, _onTurnOffSound, AudioManager.AudioType.SFX);
            }
        }

        private IEnumerator StartingSound()
        {
            AudioManager.Instance.PlaySound(_audioSource, _onTurnOnSound, AudioManager.AudioType.SFX);

            yield return new WaitForSeconds(_audioSource.pitch * _onTurnOnSound.length);

            _audioSource.clip = _onRunningSound;
            _audioSource.loop = true;
            AudioManager.Instance.PlaySource(_audioSource, AudioManager.AudioType.SFX);
        }
    }
}
