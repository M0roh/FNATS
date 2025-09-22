using System;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.PowerSystem;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Interactables
{
    public class Generator : MonoBehaviour, IInteractable, IPowerNode
    {
        public static Generator Instance { get; private set; }

        [SerializeField] private LocalizedString _interactTip;
        [Header("Light")]
        [SerializeField] private Light _powerIndicator;
        [SerializeField] private Color _offColor;
        [SerializeField] private Color _onColor;

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;

                if (_isActive)
                    _powerIndicator.color = _onColor;
                else
                    _powerIndicator.color = _offColor;
            }
        }

        public bool CanInteract => true;

        public LocalizedString InteractTip => _interactTip;

        public event Action OnBroken;
        public event Action OnRepair;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            this.RegisterNode();
            TurnOn();
        }

        private void OnDestroy()
        {
            this.UnregisterNode();
        }

        public void OnInteract()
        {
            IsActive = !IsActive;

            if (IsActive)
                TurnOn();
            else
                TurnOff();
        }

        public void OnInteractEnd() { }

        public void TurnOff()
        {
            IsActive = false;
            OnBroken?.Invoke();
        }

        public void TurnOn()
        {
            IsActive = true;
            OnRepair?.Invoke();
        }
    }
}