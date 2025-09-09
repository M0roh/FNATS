using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.UI {
    public class PauseMenu : MonoBehaviour
    {
        public static bool IsPaused { get; private set; } = false;

        private void OnEnable()
        {
            GameInput.Instance.InputActions.UI.Cancel.performed += Pause;
        }

        private void OnDisable()
        {
            GameInput.Instance.InputActions.UI.Cancel.performed += Pause;
        }

        private void Pause(InputAction.CallbackContext _)
        {
            if (IsPaused) return;

            IsPaused = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Continue()
        {
            IsPaused = false;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Settings()
        {

        }

        public void Credits()
        {

        }

        public void Quit()
        {
            LoadScreenScene.SceneToLoad = "MainMenu";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }
    }
}