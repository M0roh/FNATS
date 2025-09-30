using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoidspireStudio.FNATS.Sounds.Game
{
    public class GameSceneSounds : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip _backgroundMusic;

        [Header("Ambient")]
        [SerializeField] private List<AudioClip> _randomAmbientSounds;
        [SerializeField] private float _minAmbientDelay;
        [SerializeField] private float _maxAmbientDelay;

        private void OnEnable()
        {
            AudioManager.Instance.PlayLoopMusic(_backgroundMusic);
            StartCoroutine(RandomAmbient());
        }

        private IEnumerator RandomAmbient()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_minAmbientDelay, _maxAmbientDelay));

                var randomClip = _randomAmbientSounds[Random.Range(0, _randomAmbientSounds.Count)];
                AudioManager.Instance.PlaySound2D(randomClip, AudioManager.AudioType.Ambient);
            }
        }
    }
}
