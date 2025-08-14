using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace VoidspireStudio.FNATS.UI
{
    public class UILayoutAutoRefresher : MonoBehaviour
    {
        [SerializeField] float _waitForLocalizedTimeout = 0.5f;

        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            StartCoroutine(RefreshCoroutine());
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
        {
            StartCoroutine(RefreshCoroutine());
        }

        private IEnumerator RefreshCoroutine()
        {
            yield return null;
            yield return null;

            Canvas.ForceUpdateCanvases();

            var allTmp = GetComponentsInChildren<TMP_Text>(true);

            string[] oldTexts = new string[allTmp.Length];
            for (int i = 0; i < allTmp.Length; i++) oldTexts[i] = allTmp[i].text;

            float t = 0f;
            while (t < _waitForLocalizedTimeout)
            {
                bool changed = false;
                for (int i = 0; i < allTmp.Length; i++)
                {
                    if (allTmp[i].text != oldTexts[i])
                    {
                        changed = true;
                        break;
                    }
                }
                if (changed) break;
                t += Time.deltaTime;
                yield return null;
            }

            var csfs = GetComponentsInChildren<ContentSizeFitter>(true);
            foreach (var csf in csfs)
                csf.enabled = false;

            Canvas.ForceUpdateCanvases();

            var layoutGroups = GetComponentsInChildren<LayoutGroup>(true);
            foreach (var layout in layoutGroups)
            {
                var rect = layout.GetComponent<RectTransform>();
                if (rect != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }

            foreach (var tmp in allTmp)
                tmp.ForceMeshUpdate();


            Canvas.ForceUpdateCanvases();

            foreach (var csf in csfs)
            {
                csf.enabled = true;
                csf.SetLayoutHorizontal();
                csf.SetLayoutVertical();
            }

            Canvas.ForceUpdateCanvases();
        }
    }
}