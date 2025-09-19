using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.UI.KeyBindings
{
    public class KeyRebinding : MonoBehaviour
    {
        [Serializable]
        public class Key
        {
            public LocalizeStringEvent keyName;
            public InputActionReference keyCode;
        }

        [SerializeField] private List<Key> _keyBindings;
        [SerializeField] private GameObject _keyRowPrefab;
        [SerializeField] private Transform _keyParent;

        private void OnEnable()
        {
            GameInput.Instance.OnControlsChanged += RebuildUI;
        }

        private void OnDisable()
        {
            GameInput.Instance.OnControlsChanged -= RebuildUI;
        }

        private void RebuildUI()
        {
            foreach (Transform child in _keyParent)
                Destroy(child.gameObject);

            BuildUI();
        }

        private void BuildUI()
        {
            string currentScheme = GameInput.Instance.CurrentControlScheme;

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