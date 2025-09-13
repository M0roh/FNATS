using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.UI.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _gameUI;
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _pauseUI;

        public static bool IsPaused { get; private set; } = false;

        private void OnEnable()
        {
            GameInput.Instance.InputActions.UI.Cancel.performed += PauseProcess;
            Continue();
        }

        private void OnDisable()
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
            _pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        public void Continue()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            _gameUI.SetActive(true);
            _pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Settings()
        {
            UIManager.Instance.OpenSettings(_pauseUI);
        }

        public void Credits()
        {
        }

        public void Quit()
        {
            LoadScreenScene.SceneToLoad = "Main Menu";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }
    }
}