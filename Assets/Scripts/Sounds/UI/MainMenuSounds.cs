using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace VoidspireStudio.FNATS.Sounds.UI
{
    [DisallowMultipleComponent]
    public class MainMenuSounds : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private AudioClip _backgroundLoopMusic;
        [SerializeField] private AudioClip _TTTSahurSound;

        [Header("Ambient Delay")]
        [SerializeField] private float _minDelay = 8f;
        [SerializeField] private float _maxDelay = 25f;

        private Coroutine _loopSound;

        private void OnEnable()
        {
            AudioManager.Instance.PlayLoopMusic(_backgroundLoopMusic);
            _loopSound = StartCoroutine(LoopSound());
        }

        private void OnDisable()
        {
            if (_loopSound != null)
                StopCoroutine(_loopSound);
        }

        private IEnumerator LoopSound()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_minDelay, _maxDelay));
                AudioManager.Instance.PlaySound2D(_TTTSahurSound, AudioManager.AudioType.Ambient);
                yield return new WaitForSeconds(_TTTSahurSound.length);
            }
        }
    }
}