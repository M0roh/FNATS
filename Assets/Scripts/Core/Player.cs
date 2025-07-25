using QuickOutline;
using UnityEngine;
using UnityEngine.InputSystem;
using VoidspireStudio.FNATS.Input;
using VoidspireStudio.FNATS.Interactables;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.Core
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        private CharacterController _cc;

        [Header("Движение")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _runSpeed = 5f;
        [SerializeField] private float _crouchSpeed = 1.5f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float jumpHeight = 1.2f;
        private float _speed;

        private bool _isCrouched = false;
        private bool _isRunning = false;
        private bool _isFrozen = false;

        [Header("Оборудование")]
        [SerializeField] private Light _flashlightLight;
        private bool _flashlightEnabled = true;

        [Header("Камера")]
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Vector3 _standingCamPos = new(0, 1.7f, 0);
        [SerializeField] private Vector3 _crouchingCamPos = new(0, 0.9f, 0);
        [SerializeField] private float _cameraMoveSpeed = 5f;

        [Header("Управление камерой")]
        [SerializeField] private float _verticalRotation = 0f;
        [SerializeField] private float _maxVerticalAngle = 70f;
        [SerializeField] private float _fov = 50;
        [SerializeField] private float _sprintFov = 60;

        [Header("Взаимодействия")]
        [SerializeField] private float _interactionDistance = 3f;

        private (GameObject obj, IInteractable interact) _lastHighlightedObject;

        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private Vector3 _velocity;

        private Coroutine _sprintAdjustFov;

        public bool IsCrouch => _isCrouched;
        public bool IsRunning => _isRunning;
        public bool IsFrozen => _isFrozen;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _speed = _walkSpeed;

            _cc = GetComponent<CharacterController>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            GameInput.Instance.InputActions.Player.Sprint.started += Sprint_started;
            GameInput.Instance.InputActions.Player.Sprint.canceled += Sprint_canceled;

            GameInput.Instance.InputActions.Player.Crouch.performed += Crouch_performed;

            GameInput.Instance.InputActions.Player.Jump.performed += Jump_performed;

            GameInput.Instance.InputActions.Player.Flashlight.performed += FlashlightToggle_performed;

            GameInput.Instance.InputActions.Player.Interact.started += Interact_started;
            GameInput.Instance.InputActions.Player.Interact.canceled += Interact_canceled;
        }

        private void OnDisable()
        {
            GameInput.Instance.InputActions.Player.Sprint.started -= Sprint_started;
            GameInput.Instance.InputActions.Player.Sprint.canceled -= Sprint_canceled;

            GameInput.Instance.InputActions.Player.Crouch.performed -= Crouch_performed;

            GameInput.Instance.InputActions.Player.Jump.performed -= Jump_performed;

            GameInput.Instance.InputActions.Player.Flashlight.performed -= FlashlightToggle_performed;

            GameInput.Instance.InputActions.Player.Interact.started -= Interact_started;
            GameInput.Instance.InputActions.Player.Interact.canceled -= Interact_canceled;
        }

        private void Update()
        {
            if (Camera.main != _playerCamera)
                return;

            Look();
            Move();
            RollingCheck();
            HighlightLookObject();

            Vector3 targetPos = _isCrouched ? _crouchingCamPos : _standingCamPos;
            _playerCamera.transform.localPosition = Vector3.Lerp(
                _playerCamera.transform.localPosition,
                targetPos,
                Time.deltaTime * _cameraMoveSpeed
            );
        }

        private void HighlightLookObject()
        {
            var (newObject, interactable) = FindInteractObject();

            if (newObject != _lastHighlightedObject.obj)
            {
                if (_lastHighlightedObject.obj != null)
                    if (_lastHighlightedObject.obj.TryGetComponent<Outline>(out var lastOutline))
                        lastOutline.enabled = false;

                if (newObject != null)
                    if (newObject.TryGetComponent<Outline>(out var newOutline))
                        newOutline.enabled = true;

                _lastHighlightedObject = (newObject, interactable);
            }
        }

        private void RollingCheck()
        {
            if (_cc.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle > _cc.slopeLimit)
                {
                    Vector3 pushBack = Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized;
                    _cc.Move(_walkSpeed * Time.deltaTime * pushBack);
                }
            }
        }

        private void FlashlightToggle_performed(InputAction.CallbackContext obj)
        {
            _flashlightEnabled = !_flashlightEnabled;
            _flashlightLight.gameObject.SetActive(_flashlightEnabled);
        }

        private void Sprint_canceled(InputAction.CallbackContext obj)
        {
            if (_isCrouched)
                return;

            _speed = _walkSpeed;
            _isRunning = false;

            if (_sprintAdjustFov != null)
                StopCoroutine(_sprintAdjustFov);

            _sprintAdjustFov = StartCoroutine(Util.AdjustFOV(_fov, 0.4f, _playerCamera));
        }

        private void Sprint_started(InputAction.CallbackContext obj)
        {
            if (_isCrouched)
                return;

            _speed = _runSpeed;
            _isRunning = true;

            if (_sprintAdjustFov != null)
                StopCoroutine(_sprintAdjustFov);

            _sprintAdjustFov = StartCoroutine(Util.AdjustFOV(_sprintFov, 0.4f, _playerCamera));
        }

        private void Crouch_performed(InputAction.CallbackContext obj)
        {
            if (_isCrouched)
            {
                _cc.height = 2f;
                _cc.center = new Vector3(0, 1f, 0);
                _isCrouched = false;
                _speed = _walkSpeed;
            }
            else
            {
                _cc.height = 1f;
                _cc.center = new Vector3(0, 0.5f, 0);
                _isCrouched = true;
                _speed = _crouchSpeed;

                if (_isRunning)
                    Sprint_canceled(new InputAction.CallbackContext());
            }
        }

        private void Interact_started(InputAction.CallbackContext obj)
        {
            if (_lastHighlightedObject.interact == null) return;
            _lastHighlightedObject.interact.OnInteract();
        }

        private void Interact_canceled(InputAction.CallbackContext obj)
        {
            if (_lastHighlightedObject.interact == null) return;
            _lastHighlightedObject.interact.OnInteractEnd();
        }

        private (GameObject, IInteractable) FindInteractObject()
        {
            Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, ~0))
            {
                Transform current = hit.collider.transform;

                while (current != null)
                {
                    if (current.TryGetComponent<IInteractable>(out var interactable))
                        return (current.gameObject, interactable);

                    current = current.parent;
                }
            }

            return (null, null);
        }


        private void Jump_performed(InputAction.CallbackContext ctx)
        {
            if (_cc.isGrounded && IsSlopeValid())
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
        }

        private bool IsSlopeValid()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                return slopeAngle <= _cc.slopeLimit;
            }
            return false;
        }

        private void Move()
        {
            _moveInput = GameInput.Instance.GetMovementVector();

            Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;

            _cc.Move(_speed * Time.deltaTime * move);

            if (_cc.isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            _velocity.y += _gravity * Time.deltaTime;
            _cc.Move(_velocity * Time.deltaTime);
        }


        private void Look()
        {
            _lookInput = GameInput.Instance.GetLookVector();

            float mouseX = _lookInput.x * GameSettings.MouseSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);

            float mouseY = _lookInput.y * GameSettings.MouseSensitivity * Time.deltaTime;

            _verticalRotation -= mouseY;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -_maxVerticalAngle, _maxVerticalAngle);

            _playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }

        public void ForceLookAt(Vector3 targetPosition, bool disableVerticalRotation = false)
        {
            Vector3 direction = (targetPosition - transform.position);
            if (!disableVerticalRotation)
                direction.y = 0f;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;

            Vector3 dirToTarget = (targetPosition - _playerCamera.transform.position).normalized;
            float verticalAngle = Mathf.Asin(dirToTarget.y) * Mathf.Rad2Deg;

            _verticalRotation = Mathf.Clamp(-verticalAngle, -_maxVerticalAngle, _maxVerticalAngle);
            _playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }

        public void Freeze()
        {
            if (!_isFrozen)
            {
                _isFrozen = true;
                GameInput.Instance.InputActions.Player.Disable();
            }
        }

        public void UnFreeze()
        {
            if (_isFrozen)
            {
                _isFrozen = false;
                GameInput.Instance.InputActions.Player.Enable();
            }
        }
    }
}