using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.Animatronics;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Sounds.Game
{
    [RequireComponent(typeof(AudioSource))]
    public class AnimatronicSounds : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private AnimatronicAI _animatronic;

        [Header("Audio")]
        [SerializeField] FootstepSounds _stepsSounds;
        [SerializeField] AudioClip _attackSound;
        [SerializeField] private AudioClip _phraseSound;

        [Header("Prhase Delay")]
        [SerializeField] private float _minDelay = 15f;
        [SerializeField] private float _maxDelay = 60f;

        private AudioSource _audioSource;

        private Coroutine _loopSound;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _animatronic.OnWalk += OnAnimatronicWalk;
            _animatronic.OnAttack += OnAnimatronicAttack;

            if (_phraseSound != null)
                _loopSound = StartCoroutine(LoopSound());
        }

        private void OnDisable()
        {
            _animatronic.OnWalk -= OnAnimatronicWalk;
            _animatronic.OnAttack -= OnAnimatronicAttack;
        }

        private void OnAnimatronicAttack()
        {
            AudioManager.Instance.PlaySound(_audioSource, _attackSound, AudioManager.AudioType.SFX);
        }

        private void OnAnimatronicWalk()
        {
            string materialName = Util.GetFloorMaterialName(_animatronic.transform.position);
            var clip = _stepsSounds.GetByFloorMaterial(materialName);

            AudioManager.Instance.PlaySound(_audioSource, clip, AudioManager.AudioType.SFX);
        }

        private IEnumerator LoopSound()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_minDelay, _maxDelay));
                AudioManager.Instance.PlaySound2D(_phraseSound, AudioManager.AudioType.Ambient);
                yield return new WaitForSeconds(_phraseSound.length);
            }
        }
    }
}
