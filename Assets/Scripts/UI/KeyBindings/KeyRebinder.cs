using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.UI.KeyBindings
{
    public class KeyRebinding : MonoBehaviour
    {
        [Serializable]
        public class Key
        {
            public LocalizedString keyName;
            public InputActionReference keyCode;
        }

        [SerializeField] private List<Key> _keyBindings;
        [SerializeField] private GameObject _keyRowPrefab;
        [SerializeField] private Transform _keyParent;

        private void OnEnable()
        {
            GameInput.Instance.OnControlsChanged += RebuildUI;
            RebuildUI();
        }

        private void OnDisable()
        {
            GameInput.Instance.OnControlsChanged -= RebuildUI;
        }

        private void RebuildUI(string controlScheme = null)
        {
            foreach (Transform child in _keyParent)
                Destroy(child.gameObject);

            BuildUI(controlScheme);
        }

        private void BuildUI(string controlScheme)
        {
            string currentScheme = string.IsNullOrEmpty(controlScheme) ? GameInput.Instance.CurrentControlScheme : controlScheme;

            foreach (var keyBinding in _keyBindings)
            {
                var action = keyBinding.keyCode.action;

                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];

                    if (binding.isComposite)
                        continue;

                    if (!string.IsNullOrEmpty(binding.groups) &&
                        !binding.groups.Contains(currentScheme))
                        continue;

                    var rowObj = Instantiate(_keyRowPrefab, _keyParent);
                    var row = rowObj.GetComponent<KeyBindRow>();
                    row.Setup(keyBinding, i);
                }
            }
        }
    }
}