using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public class ColliderTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onEnter;
        [SerializeField] private UnityEvent _onStay;
        [SerializeField] private UnityEvent _onExit;

        private void OnTriggerEnter(Collider other)
        {
            _onEnter?.Invoke();
        }

        private void OnTriggerStay(Collider other)
        {
            _onStay?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            _onExit?.Invoke();
        }
    }
}