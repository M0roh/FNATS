using System; 
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Interactables;
using VoidspireStudio.FNATS.Sounds;

namespace Interactables
{
    [RequireComponent(typeof(AudioSource))]
    public class ToiletSound : MonoBehaviour, IInteractable
    {
        private LocalizedString _interactTip;

        private AudioSource _audioSource;

        public LocalizedString InteractTip => _interactTip;

        public bool CanInteract => true;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void OnInteract() => AudioManager.Instance.PlaySource(_audioSource, AudioManager.AudioType.SFX);

        public void OnInteractEnd() { }
    }
}
