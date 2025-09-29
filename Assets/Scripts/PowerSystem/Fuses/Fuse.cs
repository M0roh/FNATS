using System;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.PowerSystem.Fuses
{
    public class Fuse : MonoBehaviour, IInteractable
    {
        [SerializeField] private LocalizedString _interactTip;
        [SerializeField] private Material _fuseMaterial;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _breakColor;

        public bool CanInteract => !IsActive && Player.Player.Instance.IsPickedFuse;

        public bool IsActive { get; private set; } = true;

        public LocalizedString InteractTip => _interactTip;

        public event Action OnBroken;
        public event Action OnRepair;

        public void OnInteract() => Repair();

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
