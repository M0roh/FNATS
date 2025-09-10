using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;

        private void Awake()
        {
            SaveManager.LoadGame();
        }

        private void Start()
        {
            _continueButton.interactable = SaveManager.HasPreviousSave;
        }

        public void Continue()
        {
            LoadScreenScene.SceneToLoad = "Game Scene";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }

        public void NewGame()
        {
            SaveManager.LastSavedData.lastNight = 0;
            SaveManager.SaveGame();

            LoadScreenScene.SceneToLoad = "Game Scene";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }

        public void Settings() { }

        public void Credits() { }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
