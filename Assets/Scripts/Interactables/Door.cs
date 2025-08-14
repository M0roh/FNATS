using System.Collections;
using UnityEngine;
using UnityEngine.AI;


namespace VoidspireStudio.FNATS.Interactables
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Quaternion _openRotation = new();
        [SerializeField] private Quaternion _closeRotation = new();
        [SerializeField] private float _rotationSpeed = 5f;
        // [SerializeField] private bool _isNeedHoldToClose = false;

        private bool _isOpen = true;
        private Coroutine rotateCoroutine;

        private NavMeshObstacle _navMeshObstacle;

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
        }


        private void OnEnable()
        {
            transform.localRotation = _openRotation;
            _navMeshObstacle.enabled = false;
            _isOpen = true;
        }

        public void OnInteract()
        {
            Debug.Log("Close");
            StartRotate(_closeRotation);
            _navMeshObstacle.enabled = true;

            _isOpen = false;
        }

        public void OnInteractEnd()
        {
            Debug.Log("Close");
            StartRotate(_openRotation);
            _navMeshObstacle.enabled = false;

            _isOpen = true;
        }

        private void StartRotate(Quaternion targetRot)
        {
            if (rotateCoroutine != null)
                StopCoroutine(rotateCoroutine);

            rotateCoroutine = StartCoroutine(RotateDoor(targetRot));
        }

        private IEnumerator RotateDoor(Quaternion targetRot)
        {
            while (Quaternion.Angle(transform.localRotation, targetRot) > 0.01f)
            {
                transform.localRotation = Quaternion.Slerp(
                    transform.localRotation,
                    targetRot,
                    _rotationSpeed * Time.deltaTime
                );
                Debug.Log(transform.localRotation);
                yield return null;
            }
            transform.localRotation = targetRot;
        }

    }
}