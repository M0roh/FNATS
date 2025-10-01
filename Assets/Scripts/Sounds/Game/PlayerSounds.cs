using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.Player;
using VoidspireStudio.FNATS.Sounds;

namespace Sounds.Game
{
    [AddComponentMenu("Sounds/Game/PlayerSounds")]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private AudioClip _stepSound;
        [SerializeField] private AudioClip _jumpSound;
        [SerializeField] private AudioClip _onGrounding;

        private bool _isNeedPlay = false;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            Player.Instance.OnWalk += OnPlayerWalk;
            Player.Instance.OnRunStateChange += OnPlayerRunStateChange;
            Player.Instance.OnCrouchStateChange += OnPlayerCrouchStateChange; ;
            Player.Instance.OnJump += OnPlayerJump;
            Player.Instance.OnGrouding += OnPlayerGrounding;
            StartCoroutine(PlayMoveSoundCooldown());
        }

        private void OnPlayerCrouchStateChange(bool isCrouched)
        {
            if (isCrouched)
                _audioSource.pitch /= 2f;
            else
                _audioSource.pitch *= 2f;
        }

        private void OnDisable()
        {
            Player.Instance.OnWalk -= OnPlayerWalk;
            Player.Instance.OnRunStateChange -= OnPlayerRunStateChange;
            Player.Instance.OnJump -= OnPlayerJump;
            Player.Instance.OnGrouding -= OnPlayerGrounding;
        }

        private void OnPlayerWalk()
        {
            _isNeedPlay = true;
        }

        private IEnumerator PlayMoveSoundCooldown()
        {
            while (true)
            {
                if (_isNeedPlay)
                {
                    AudioManager.Instance.PlaySound(_audioSource, _stepSound, AudioManager.AudioType.SFX);
                    _isNeedPlay = false;
                    yield return new WaitForSeconds(_stepSound.length * _audioSource.pitch);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void OnPlayerRunStateChange(bool isRunning)
        {
            if (isRunning)
                _audioSource.pitch *= 2f;
            else
                _audioSource.pitch /= 2f;
        }

        private void OnPlayerJump() => AudioManager.Instance.PlaySound(_audioSource, _jumpSound, AudioManager.AudioType.SFX);

        private void OnPlayerGrounding() => AudioManager.Instance.PlaySound(_audioSource, _onGrounding, AudioManager.AudioType.SFX);
    }
}
