using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.Sounds.Game
{
    [RequireComponent(typeof(AudioSource))]
    public class MachineSounds : SerializedMonoBehaviour
    {
        [OdinSerialize] private IMachineEvents _machine;

        [Header("Audio")]
        [SerializeField] private AudioClip _onTurnOnSound;
        [SerializeField] private AudioClip _onTurnOffSound;
        [SerializeField] private AudioClip _onRunningSound;
        [SerializeField] private AudioClip _onBrokenSound;
        private AudioSource _audioSource;

        private Coroutine _soundsStarting;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _machine.OnActiveChange += OnActiveChange;
            _machine.OnBroken += OnBroken;
        }

        private void OnBroken()
        {
            if (_onBrokenSound != null)
                AudioManager.Instance.PlaySound(_audioSource, _onBrokenSound);
        }

        private void OnDisable()
        {
            _machine.OnActiveChange -= OnActiveChange;
        }

        private void OnActiveChange(bool isActive)
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

                AudioManager.Instance.PlaySound(_audioSource, _onTurnOffSound);
            }
        }

        private IEnumerator StartingSound()
        {
            AudioManager.Instance.PlaySound(_audioSource, _onTurnOnSound);

            yield return new WaitForSeconds(_audioSource.pitch * _onTurnOnSound.length);

            _audioSource.clip = _onRunningSound;
            _audioSource.loop = true;
            AudioManager.Instance.PlaySource(_audioSource);
        }
    }
}
