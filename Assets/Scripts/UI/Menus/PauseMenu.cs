using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.UI.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Button _firstSelectedBtn;

        private void OnEnable()
        {
            GameInput.Instance.OnControlsChanged += OnControlsChanged;
            OnControlsChanged(GameInput.Instance.CurrentControlScheme);
        }

        private void OnDisable()
        {
            GameInput.Instance.OnControlsChanged -= OnControlsChanged;
        }

        private void OnControlsChanged(string controlScheme)
        {
            if (controlScheme.Equals("Gamepad", System.StringComparison.OrdinalIgnoreCase)
                || controlScheme.Equals("Joystick", System.StringComparison.OrdinalIgnoreCase))
            {
                EventSystem.current.SetSelectedGameObject(_firstSelectedBtn.gameObject);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void Continue()
        {
            PauseManager.Instance.Continue();
        }

        public void Settings()
        {
            UIManager.Instance.OpenSettings(gameObject);
        }

        public void Credits()
        {
            UIManager.Instance.OpenCredits(gameObject);
        }

        public void Quit()
        {
            LoadScreenScene.SceneToLoad = "Main Menu";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }
    }
}