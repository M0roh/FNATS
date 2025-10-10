using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Sounds;
using VoidspireStudio.FNATS.Utils;


namespace VoidspireStudio.FNATS.Interactables
{
    [RequireComponent(typeof(NavMeshObstacle), typeof(AudioSource))]
    public class Door : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        [SerializeField] private LocalizedString _interactTip;
        [SerializeField] private bool _isOpenOnStart = false;
        [SerializeField] private bool _isNeedHold = false;
        [SerializeField] private float _rotationSpeed = 5f;

        [Header("Angles")]
        [SerializeField] private Quaternion _closeAngle = new();
        [SerializeField] private Quaternion _openAngle = new();

        [Header("Audio")]
        [SerializeField] private AudioClip _openCloseSound;
        [SerializeField] private AudioClip _breakSound;

        private AudioSource _audioSource;
        private NavMeshObstacle _obstacle;

        private CancellationTokenSource _rotateCoroutineCancelToken;

        private bool _isOpen = true;
        private bool _isBroken = false;
        
        public bool IsOpen => _isOpen;
        public bool IsBroken => _isBroken;

        public bool CanInteract => !_isBroken;

        public LocalizedString InteractTip => _interactTip;

        private void Awake()
        {
            _obstacle = GetComponent<NavMeshObstacle>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (_isOpenOnStart)
                Open().Forget();
            else
                Close().Forget();
        }

        public void OnInteract()
        {
            if (_isBroken) return;

            if (_isNeedHold)
            {
                Close().Forget();
                return;
            }

            if (IsOpen)
                Close().Forget();
            else
                Open().Forget();
        }

        public void OnInteractEnd()
        {
            if (_isBroken) return;

            if (_isNeedHold)
                Open().Forget();
        }

        public async UniTask Close()
        {
            if (_rotateCoroutineCancelToken?.Token.CanBeCanceled ?? false)
            {
                _rotateCoroutineCancelToken.Cancel();
            }
            _rotateCoroutineCancelToken = CancellationTokenSource.CreateLinkedTokenSource(new(), this.GetCancellationTokenOnDestroy());

            try
            {
                await transform.Rotate(_closeAngle, _rotationSpeed).SuppressCancellationThrow();
            }
            finally
            {
                _rotateCoroutineCancelToken?.Dispose();
                _rotateCoroutineCancelToken = null;
            }

            _isOpen = false;
            _obstacle.enabled = true;

            AudioManager.Instance.PlaySound(_audioSource, _openCloseSound);
        }

        public async UniTask Open()
        {
            if (_rotateCoroutineCancelToken?.Token.CanBeCanceled ?? false)
            {
                _rotateCoroutineCancelToken.Cancel();
            }
            _rotateCoroutineCancelToken = CancellationTokenSource.CreateLinkedTokenSource(new(), this.GetCancellationTokenOnDestroy());

            try
            {
                await transform.Rotate(_openAngle, _rotationSpeed).SuppressCancellationThrow();
            }
            finally
            {
                _rotateCoroutineCancelToken?.Dispose();
                _rotateCoroutineCancelToken = null;
            }

            _isOpen = true;
            _obstacle.enabled = false;

            AudioManager.Instance.PlaySound(_audioSource, _openCloseSound);
        }

        public void Break()
        {
            Open().Forget();
            _isBroken = true;

            AudioManager.Instance.PlaySound(_audioSource, _breakSound);
        }
    }
}