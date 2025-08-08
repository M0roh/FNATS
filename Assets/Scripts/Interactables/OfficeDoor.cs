using UnityEngine;
using UnityEngine.AI;


namespace VoidspireStudio.FNATS.Interactables
{
    [RequireComponent(typeof(Animator))]
    public class OfficeDoor : MonoBehaviour, IInteractable
    {
        private Animator _animator;
        private NavMeshObstacle _navMeshObstacle;

        private bool _isOpen = true;

        private const string OPEN = "OPEN";
        private const string CLOSE= "CLOSE";

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
        }

        private void OnEnable()
        {
            OnInteractEnd();
        }

        public void OnInteract()
        {
            _animator.ResetTrigger(OPEN);
            _animator.SetTrigger(CLOSE);
            _navMeshObstacle.enabled = true;
 
            _isOpen = false;
        }

        public void OnInteractEnd()
        {
            _animator.ResetTrigger(CLOSE);
            _animator.SetTrigger(OPEN);
            _navMeshObstacle.enabled = false;
            
            _isOpen = true;
        }
    }
}