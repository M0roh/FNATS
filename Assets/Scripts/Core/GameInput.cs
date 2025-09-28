using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.Core
{
    [RequireComponent(typeof(PlayerInput))]
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; protected set; }

        [SerializeField] private InputSystem_Actions _inputActions;
        private PlayerInput _playerInput;

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "InputBindingSave.sav");

        public InputSystem_Actions InputActions => _inputActions;

        public string CurrentControlScheme => _playerInput?.currentControlScheme;

        public event System.Action<string> OnControlsChanged;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _inputActions = new InputSystem_Actions();

            _playerInput = GetComponent<PlayerInput>();
            _playerInput.actions = _inputActions.asset;

            InputUser.onChange += InputUser_onChange;

            LoadBindings();

            _inputActions.Player.Enable();
            _inputActions.UI.Enable();
        }

        private void OnDestroy()
        {
            InputUser.onChange -= InputUser_onChange;
        }

        private void InputUser_onChange(InputUser _, InputUserChange __, InputDevice ___) => OnControlsChanged?.Invoke(_playerInput.currentControlScheme);

        public Vector2 GetMovementVector()
        {
            return _inputActions.Player.Move.ReadValue<Vector2>();
        }

        public Vector2 GetLookVector()
        {
            return _inputActions.Player.Look.ReadValue<Vector2>();
        }

        public Vector2 GetCameraLookVector()
        {
            return _inputActions.Camera.Look.ReadValue<Vector2>();
        }

        public void SaveBindings()
        {
            string inputMap = InputActions.SaveBindingOverridesAsJson();
            byte[] bindingsData = SaveManager.EncodeData(inputMap);
            File.WriteAllBytes(SaveFilePath, bindingsData);
        }

        public void LoadBindings()
        {
            if (!File.Exists(SaveFilePath)) return;

            byte[] bindingsData = File.ReadAllBytes(SaveFilePath);
            string inputMap = SaveManager.DecodeData(bindingsData);

            InputActions.Disable();
            InputActions.LoadBindingOverridesFromJson(inputMap);
            InputActions.Enable();
        }
    }
}