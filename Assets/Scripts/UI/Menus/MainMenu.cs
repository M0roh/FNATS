using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;

        private bool _isOpen = true;

        private void Awake()
        {
            SaveManager.LoadGame();
        }

        private void Start()
        {
            _continueButton.interactable = SaveManager.HasPreviousSave;
        }

        private void OnEnable()
        {
            _isOpen = true;
        }

        public void Continue()
        {
            if (!_isOpen) return;

            LoadScreenScene.SceneToLoad = "Game Scene";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }

        public void NewGame()
        {
            if (!_isOpen) return;

            SaveManager.LastSavedData.lastNight = 0;
            SaveManager.SaveGame();

            LoadScreenScene.SceneToLoad = "Game Scene";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }

        public void Settings()
        {
            if (!_isOpen) return;

            _isOpen = false;
            UIManager.Instance.OpenSettings(gameObject);
        }

        public void Credits()
        {
            if (!_isOpen) return;
        }

        public void Quit()
        {
            if (!_isOpen) return;

            Application.Quit();
        }
    }
}
