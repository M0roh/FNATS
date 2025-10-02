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
        [Header("Audio")]
        [SerializeField] private AudioClip _stepSound;
        [SerializeField] private AudioClip _jumpSound;
        [SerializeField] private AudioClip _onGrounding;

        [Header("Settings")]
        [SerializeField] private float _basePitch = 1f;
        [SerializeField] private float _baseStepDelay = 1f;
        [SerializeField] private float _stepFactor = 0.3f;

        private bool _isNeedPlay = false;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            Player.Instance.OnWalk += OnPlayerWalk;
            Player.Instance.OnJump += OnPlayerJump;
            Player.Instance.OnGrouding += OnPlayerGrounding;
            StartCoroutine(PlayMoveSoundCooldown());
        }

        private void OnDisable()
        {
            Player.Instance.OnWalk -= OnPlayerWalk;
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
                    _isNeedPlay = false;

                    float speed = Player.Instance.Speed;

                    if (speed <= 0.1f || !Player.Instance.IsGrounded)
                    {
                        yield return null;
                        continue;
                    }

                    AudioManager.Instance.PlaySound(_audioSource, _stepSound, AudioManager.AudioType.SFX);

                    float delay = _stepFactor / speed;

                    yield return new WaitForSeconds(delay);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void OnPlayerJump() => AudioManager.Instance.PlaySound(_audioSource, _jumpSound, AudioManager.AudioType.SFX);

        private void OnPlayerGrounding() => AudioManager.Instance.PlaySound(_audioSource, _onGrounding, AudioManager.AudioType.SFX);
    }
}
