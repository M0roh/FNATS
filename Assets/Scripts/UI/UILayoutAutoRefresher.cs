using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace VoidspireStudio.FNATS.UI
{
    public class UILayoutAutoRefresher : MonoBehaviour
    {
        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged(Locale locale)
        {
            StartCoroutine(RefreshLayoutsNextFrame());
        }

        private IEnumerator RefreshLayoutsNextFrame()
        {
            yield return null;

            var layoutGroups = GetComponentsInChildren<LayoutGroup>(true);
            foreach (var layout in layoutGroups)
            { 
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
            }
        }
    }
}