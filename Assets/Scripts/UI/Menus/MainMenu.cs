using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus
{
    [RequireComponent(typeof(Animator))]
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu Instance { get; private set; }

        [SerializeField] private List<Animator> _animatronics = new();
        [SerializeField] private Button _continueButton;
        [SerializeField] private Volume _globalVolume;

        public Volume GlobalVolume => _globalVolume;

        private void Awake()
        {
            Instance = this;
            SaveManager.LoadGame();

            _animatronics.ForEach(animatronic => animatronic.SetTrigger("Dance"));
        }

        private void Start()
        {
            if (_globalVolume.profile.TryGet<Exposure>(out var exp))
                exp.fixedExposure.value = (SaveManager.LastSavedData.graphics.brightness * -8f) + 4f;

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
