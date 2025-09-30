using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus
{
    [RequireComponent(typeof(Animator))]
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu Instance { get; private set; }

        [SerializeField] private List<Animator> _animatronics = new();
        
        [Header("UI")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _firstSelectedBtn;

        [Header("Other")]
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
