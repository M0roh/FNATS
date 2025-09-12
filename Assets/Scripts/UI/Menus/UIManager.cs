using UnityEngine;

namespace VoidspireStudio.FNATS.UI.Menus
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Меню")]
        [SerializeField] private GameObject _settings;
        [SerializeField] private GameObject _credits;

        private GameObject _previousMenu;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void OpenSettings(GameObject fromMenu)
        {
            fromMenu.SetActive(false);
            Instantiate(_settings, fromMenu.transform.parent);
            _previousMenu = fromMenu;
        }

        public void OpenCredits(GameObject fromMenu)
        {
            fromMenu.SetActive(false);
            Instantiate(_credits, fromMenu.transform.parent);
            _previousMenu = fromMenu;
        }

        public void BackToMenu(GameObject fromMenu)
        {
            if (_previousMenu == null) return;

            fromMenu.SetActive(false);
            _previousMenu.SetActive(true);

            Destroy(fromMenu);
        }
    }
}
