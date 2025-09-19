using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Animatronics;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus
{
    [RequireComponent(typeof(Animator))]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private List<Animator> _animatronics = new();
        [SerializeField] private Button _continueButton;

        private void Awake()
        {
            SaveManager.LoadGame();

            _animatronics.ForEach(animatronic => animatronic.SetTrigger("Dance"));
        }

        private void Start()
        {
            _continueButton.interactable = SaveManager.HasSavedGame;
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
