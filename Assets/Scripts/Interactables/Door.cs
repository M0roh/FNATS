using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Localization;


namespace VoidspireStudio.FNATS.Interactables
{
    [RequireComponent(typeof(NavMeshObstacle))]
    public class Door : MonoBehaviour, IInteractable
    {                                                                                 
        [SerializeField] private bool _isOpenOnStart = false;
        [SerializeField] private bool _isNeedHold = false;
        [SerializeField] private Quaternion _closeAngle = new();
        [SerializeField] private Quaternion _openAngle = new();
        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private LocalizedString _interactTip;

        private NavMeshObstacle _obstacle;
        private Coroutine _rotateCoroutine;

        private bool _isOpen = true;
        private bool _isBroken = false;
        
        public bool IsOpen => _isOpen;
        public bool IsBroken => _isBroken;

        public bool CanInteract => !_isBroken;

        public LocalizedString InteractTip => _interactTip;

        private void Awake()
        {
            _obstacle = GetComponent<NavMeshObstacle>();
        }

        private void Start()
        {
            if (_isOpenOnStart)
                Open();
            else
                Close();
        }

        public void OnInteract()
        {
            if (_isBroken) return;

            if (_isNeedHold)
                Close();
            else if (IsOpen)
                Close();
            else
                Open();
        }

        public void OnInteractEnd()
        {
            if (_isBroken) return;

            if (_isNeedHold)
                Open();
        }

        public void Close()
        {
            if (_rotateCoroutine != null) StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = StartCoroutine(Rotate(_closeAngle));

            _isOpen = false;
            _obstacle.enabled = true;
        }

        public void Open()
        {
            if (_rotateCoroutine != null) StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = StartCoroutine(Rotate(_openAngle));

            _isOpen = true;
            _obstacle.enabled = false;
        }

        public void Break()
        {
            Open();
            _isBroken = true;
        }

        public IEnumerator Rotate(Quaternion targetAngle) 
        {
            while (Quaternion.Angle(transform.localRotation, targetAngle) > 0.01f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetAngle, _rotationSpeed * Time.deltaTime);

                yield return null;
            }
            transform.localRotation = targetAngle;
        }
    }
}