using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Interactables;
using VoidspireStudio.FNATS.Sounds;

namespace VoidspireStudio.FNATS.PowerSystem.Fuses
{
    [RequireComponent(typeof(AudioSource))]
    public class FuseToolBox : MonoBehaviour, IInteractable
    {
        [SerializeField] private LocalizedString _interactTip;
        [SerializeField] private AudioClip _pickupSound;
        [SerializeField] private List<GameObject> _fusesInBox = new();

        private AudioSource _audioSource;

        public bool CanInteract => true;

        public LocalizedString InteractTip => _interactTip;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void OnInteract()
        {
            if (!Player.Player.Instance.IsPickedFuse && _fusesInBox.Count > 0)
            {
                Player.Player.Instance.IsPickedFuse = true;
                AudioManager.Instance.PlaySound(_audioSource, _pickupSound, AudioManager.AudioType.SFX);

                var fuse = _fusesInBox.Last();
                Destroy(fuse);
                _fusesInBox.Remove(fuse);

            }
        }

        public void OnInteractEnd() { }
    }
}
