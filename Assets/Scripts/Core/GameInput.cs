using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.Core
{
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; protected set; }

        private InputSystem_Actions _inputActions;
        private PlayerInput _playerInput;

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "InputBindingSave.sav");

        public InputSystem_Actions InputActions => _inputActions;

        public string CurrentControlScheme => _playerInput?.currentControlScheme;

        public event System.Action OnControlsChanged;

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
            _playerInput = gameObject.AddComponent<PlayerInput>();
            _playerInput.actions = _inputActions.asset;
            _playerInput.defaultControlScheme = _inputActions.controlSchemes[0].name;
            _playerInput.neverAutoSwitchControlSchemes = false;

            _playerInput.onControlsChanged += ctx =>
            {
                Debug.Log($"Control scheme changed: {_playerInput.currentControlScheme}");
                OnControlsChanged?.Invoke();
            };

            LoadBindings();

            _inputActions.Player.Enable();
            _inputActions.UI.Enable();
        }

        public Vector2 GetMovementVector()
        {
            return _inputActions.Player.Move.ReadValue<Vector2>();
        }

        public Vector2 GetLookVector()
        {
            return _inputActions.Player.Look.ReadValue<Vector2>();
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