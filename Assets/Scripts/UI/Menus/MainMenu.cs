using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus
{
    [RequireComponent(typeof(Animator))]
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
            Application.Quit();
        }
    }
}
