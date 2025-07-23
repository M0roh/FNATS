using UnityEngine;
using UnityEngine.Events;

namespace VoidspireStudio.FNATS.Triggers
{
    public class ColliderTrigger : MonoBehaviour
    {
        [SerializeField] protected UnityEvent _onEnter;
        [SerializeField] protected UnityEvent _onStay;
        [SerializeField] protected UnityEvent _onExit;

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