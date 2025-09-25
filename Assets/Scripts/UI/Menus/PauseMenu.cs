using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.UI.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _gameUI;
        [SerializeField] private GameObject _pauseButtons;

        public static bool IsPaused { get; private set; } = false;

        private void Start()
        {
            Continue();
            GameInput.Instance.InputActions.UI.Cancel.performed += PauseProcess;
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

        private void Pause()
        {
            if (IsPaused) return;

            IsPaused = true;
            Time.timeScale = 0f;
            _gameUI.SetActive(false);
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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

        public void Settings()
        {
            UIManager.Instance.OpenSettings(_pauseButtons);
        }

        public void Credits()
        {
            UIManager.Instance.OpenCredits(_pauseButtons);
        }

        public void Quit()
        {
            LoadScreenScene.SceneToLoad = "Main Menu";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }
    }
}