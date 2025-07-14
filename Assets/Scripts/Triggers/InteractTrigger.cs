using Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class InteractTrigger : MonoBehaviour, IInteractable
    {
        [Header("Взаимодействие")]
        [SerializeField] private UnityEvent _onInteract;

        [Header("Взаимодействие с зажатием")]
        [SerializeField] private UnityEvent _onShortInteract;
        [SerializeField] private UnityEvent _onInteractHold;
        [SerializeField] private float _requiredHoldTime = 0.5f;

        private float _pressTime; 

        public void OnInteract()
        {
            _onInteract?.Invoke();
            _pressTime = Time.time;
        }

        public void OnInteractEnd()
        {
            float heldTime = Time.time - _pressTime;

            if (heldTime >= _requiredHoldTime)
                _onInteractHold?.Invoke();
            else
                _onShortInteract?.Invoke();
        }
    }
}