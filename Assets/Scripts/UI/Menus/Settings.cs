using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus
{
    [RequireComponent(typeof(Animator))]
    public class Settings : MonoBehaviour
    {
        [System.Serializable]
        public class SettingsTab
        {
            [NotNull] public GameObject tabPanel;
            [NotNull] public Button tabButton;
            public UnityAction clickAction;
        }

        [Header("Tabs")]
        [SerializeField] private List<SettingsTab> _tabs = new();
        private Transform _tabParent;
        private int _currentTab = 0;

        [Header("Default Settings")]
        [SerializeField] private float _clickedButtonScale = 1.1f;
        private Vector2 _normalButtonSize = Vector2.zero;
        private Vector2 _clickedButtonSize = Vector2.zero;

        [Header("Current Tab Settings")]
        [SerializeField] private int _defaultButtonFontSize = 40;
        [SerializeField] private int _clickedButtonFontSize = 50;

        private void Awake()
        {
            _normalButtonSize = (_tabs[0].tabButton.transform as RectTransform).sizeDelta;
            _clickedButtonSize = _normalButtonSize * _clickedButtonScale;
            _tabParent = _tabs[0].tabButton.transform.parent;
        }

        public void Start()
        {
            ChangeTab(_currentTab);
        }

        private void OnEnable()
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                int index = i;
                _tabs[i].clickAction = () => ChangeTab(index);
                _tabs[i].tabButton.onClick.AddListener(_tabs[i].clickAction);
            }

            GameInput.Instance.OnControlsChanged += OnControlsChanged;
            OnControlsChanged(GameInput.Instance.CurrentControlScheme);
        }

        private void OnDisable()
        {
            GameInput.Instance.OnControlsChanged -= OnControlsChanged;

            for (int i = 0; i < _tabs.Count; i++)
                _tabs[i].tabButton.onClick.RemoveListener(_tabs[i].clickAction);
        }

        private void OnControlsChanged(string controlScheme)
        {
            if (controlScheme.Equals("Gamepad", System.StringComparison.OrdinalIgnoreCase)
                || controlScheme.Equals("Joystick", System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log(_tabs[0].tabButton.gameObject);
                EventSystem.current.SetSelectedGameObject(_tabParent.GetComponentsInChildren<Button>()[0].gameObject);
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

        public void ChangeTab(int tabIndex)
        {
            var newTab = _tabs[tabIndex];
            newTab.tabPanel.SetActive(true);
            (newTab.tabButton.transform as RectTransform).sizeDelta = _clickedButtonSize;
            newTab.tabButton.GetComponentInChildren<TMP_Text>().fontSize = _clickedButtonFontSize;
            newTab.tabButton.interactable = false;

            foreach (var tab in _tabs.Where(tab => tab != newTab))
            {
                tab.tabPanel.SetActive(false);
                (tab.tabButton.transform as RectTransform).sizeDelta = _normalButtonSize;
                tab.tabButton.GetComponentInChildren<TMP_Text>().fontSize = _defaultButtonFontSize;
                tab.tabButton.interactable = true;
            }

            _currentTab = tabIndex;
            EventSystem.current.SetSelectedGameObject(_tabParent.GetComponentsInChildren<Button>()[0].gameObject);
        }

        public void BackButton()
        {
            SaveManager.SaveGame();
            GameInput.Instance.SaveBindings();
            UIManager.Instance.BackToMenu(gameObject);
        }
    }
}
