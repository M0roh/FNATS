using System;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Interactables;
using VoidspireStudio.FNATS.Sounds;

namespace VoidspireStudio.FNATS.PowerSystem.Fuses
{
    [RequireComponent(typeof(AudioSource))]
    public class Fuse : MonoBehaviour, IInteractable
    {
        [Header("Interact")]
        [SerializeField] private LocalizedString _interactTip;
        [SerializeField] private AudioClip _insertSound;
        private AudioSource _audioSource;
        
        [Header("Setup")]
        [SerializeField] private Material _fuseMaterial;

        [Header("Colors")]
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _breakColor;

        public bool CanInteract => !IsActive && Player.Player.Instance.IsPickedFuse;

        public bool IsActive { get; private set; } = true;

        public LocalizedString InteractTip => _interactTip;

        public event Action OnBroken;
        public event Action OnRepair;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void OnInteract()
        {
            Repair();
            AudioManager.Instance.PlaySound(_audioSource, _insertSound, AudioManager.AudioType.SFX);
        }

        public void Repair()
        {
            if (!Player.Player.Instance.IsPickedFuse) return;

            IsActive = true;
            _fuseMaterial.color = _normalColor;
            OnRepair?.Invoke();
        }

        public void Break()
        {
            IsActive = false;
            _fuseMaterial.color = _breakColor;
            OnBroken?.Invoke();
        }

        public void OnInteractEnd() { }
    }
}
