using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
            public string tabName;
            public UnityAction clickAction;
        }

        [Header("Tabs")]
        [SerializeField] private List<SettingsTab> _tabs = new();
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
                _tabs[i].tabName = _tabs[i].tabButton.GetComponentInChildren<TMP_Text>().text;
                _tabs[i].clickAction = () => ChangeTab(index);
                _tabs[i].tabButton.onClick.AddListener(_tabs[i].clickAction);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _tabs.Count; i++)
                _tabs[i].tabButton.onClick.RemoveListener(_tabs[i].clickAction);
        }

        public void ChangeTab(int tabIndex)
        {
            var newTab = _tabs[tabIndex];
            newTab.tabPanel.SetActive(true);
            (newTab.tabButton.transform as RectTransform).sizeDelta = _clickedButtonSize;
            newTab.tabButton.GetComponentInChildren<TMP_Text>().text = $"<b>{newTab.tabName}</b>";
            newTab.tabButton.GetComponentInChildren<TMP_Text>().fontSize = _clickedButtonFontSize;
            newTab.tabButton.interactable = false;

            foreach (var tab in _tabs.Where(tab => tab != newTab))
            {
                tab.tabPanel.SetActive(false);
                (tab.tabButton.transform as RectTransform).sizeDelta = _normalButtonSize;
                tab.tabButton.GetComponentInChildren<TMP_Text>().text = tab.tabName;
                tab.tabButton.GetComponentInChildren<TMP_Text>().fontSize = _defaultButtonFontSize;
                tab.tabButton.interactable = true;
            }

            _currentTab = tabIndex;
        }

        public void BackButton()
        {
            SaveManager.SaveGame();
            UIManager.Instance.BackToMenu(gameObject);
        }
    }
}
