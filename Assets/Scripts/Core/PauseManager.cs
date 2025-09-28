using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VoidspireStudio.FNATS.UI.Menus;

namespace VoidspireStudio.FNATS.Core
{
    public class PauseManager : MonoBehaviour
    {
        public static PauseManager Instance { get; private set; }
        public static bool IsPaused { get; private set; } = false;

        [SerializeField] private GameObject _gameUI;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameInput.Instance.InputActions.UI.Cancel.performed += PauseProcess;
            Continue();
        }

        private void OnDestroy()
        {
            GameInput.Instance.InputActions.UI.Cancel.performed -= PauseProcess;
        }

        private void PauseProcess(InputAction.CallbackContext _)
        {
            if (IsPaused)
                Continue();
            else
                Pause();
        }

        public void Pause()
        {
            if (IsPaused) return;

            IsPaused = true;
            Time.timeScale = 0f;
            _gameUI.SetActive(false);
            gameObject.SetActive(true);

            if (!GameInput.Instance.CurrentControlScheme.Equals("Gamepad", System.StringComparison.OrdinalIgnoreCase)
                && !GameInput.Instance.CurrentControlScheme.Equals("Joystick", System.StringComparison.OrdinalIgnoreCase))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void Continue()
        {
            if (UIManager.Instance.IsOpenedSubMenu) return;

            IsPaused = false;
            Time.timeScale = 1f;
            _gameUI.SetActive(true);
            gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
