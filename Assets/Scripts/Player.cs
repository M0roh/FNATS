using UnityEngine;
using UnityEngine.InputSystem;

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

    private bool _isJumping = false;
    private bool _isCrouched = false;
    private bool _isRunning = false;

    [Header("Оборудование")]
    [SerializeField] private Light _flashlight;
    private bool _flashlightEnabled = true;

    [Header("Камера")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _verticalRotation = 0f;
    [SerializeField] private float _maxVerticalAngle = 70f;
    [SerializeField] private float _fov = 50;
    [SerializeField] private float _sprintFov = 60;
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private Vector3 _standingCamPos = new(0, 1.7f, 0);
    [SerializeField] private Vector3 _crouchingCamPos = new(0, 0.9f, 0);
    [SerializeField] private float _cameraMoveSpeed = 5f;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector3 _velocity;

    private Coroutine _sprintAdjustFov;

    private void Awake()
    {
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
    }

    private void OnDisable()
    {
        GameInput.Instance.InputActions.Player.Sprint.started -= Sprint_started;
        GameInput.Instance.InputActions.Player.Sprint.canceled -= Sprint_canceled;

        GameInput.Instance.InputActions.Player.Crouch.performed -= Crouch_performed;

        GameInput.Instance.InputActions.Player.Jump.performed -= Jump_performed;

        GameInput.Instance.InputActions.Player.Flashlight.performed -= FlashlightToggle_performed;
    }

    private void Update()
    {
        if (Camera.main != _playerCamera)
            return;

        Look();
        Move();

        Vector3 targetPos = _isCrouched ? _crouchingCamPos : _standingCamPos;
        _playerCamera.transform.localPosition = Vector3.Lerp(
            _playerCamera.transform.localPosition,
            targetPos,
            Time.deltaTime * _cameraMoveSpeed
        );
    }

    private void FlashlightToggle_performed(InputAction.CallbackContext obj)
    {
        _flashlightEnabled = !_flashlightEnabled;
        _flashlight.gameObject.SetActive(_flashlightEnabled);
    }

    private void Sprint_canceled(InputAction.CallbackContext obj)
    {
        if (_isCrouched)
            return;

        _speed = _walkSpeed;
        _isRunning = false;

        if (_sprintAdjustFov != null)
            StopCoroutine(_sprintAdjustFov);

        _sprintAdjustFov = StartCoroutine(Utils.AdjustFOV(_fov, 0.4f, _playerCamera));
    }

    private void Sprint_started(InputAction.CallbackContext obj)
    {
        if (_isCrouched)
            return;

        _speed = _runSpeed;
        _isRunning = true;

        if (_sprintAdjustFov != null)
            StopCoroutine(_sprintAdjustFov);

        _sprintAdjustFov = StartCoroutine(Utils.AdjustFOV(_sprintFov, 0.4f, _playerCamera));
    }

    protected void Crouch_performed(InputAction.CallbackContext obj)
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

    private void Jump_performed(InputAction.CallbackContext ctx)
    {
        if (_cc.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
            _isJumping = true;
        }
    }

    private void Move()
    {
        _moveInput = GameInput.Instance.GetMovementVector();

        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        _cc.Move(move * _speed * Time.deltaTime);

        if (_cc.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _velocity.y += _gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }


    private void Look()
    {
        _lookInput = GameInput.Instance.GetLookVector();

        float mouseX = _lookInput.x * _mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = _lookInput.y * _mouseSensitivity * Time.deltaTime;

        _verticalRotation -= mouseY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_maxVerticalAngle, _maxVerticalAngle);

        _playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }
}