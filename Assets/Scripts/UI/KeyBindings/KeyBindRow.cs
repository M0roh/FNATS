using TMPro;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.UI.KeyBindings
{
    public class KeyBindRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _inputName;
        [SerializeField] private TMP_Text _keyName;

        private InputAction _action;
        private int _bindingIndex;

        public void Setup(InputAction action, int bindingIndex, string displayNameOverride = null)
        {
            _action = action;
            _bindingIndex = bindingIndex;

            var binding = action.bindings[bindingIndex];

            _inputName.text = (!string.IsNullOrEmpty(displayNameOverride) ? displayNameOverride : binding.isPartOfComposite ? binding.name : action.name) + ":";

            _keyName.text = action.GetBindingDisplayString(bindingIndex);
        }

        public async void Setup(KeyRebinding.Key key, int bindingIndex)
        {
            var action = GameInput.Instance.InputActions.FindAction(key.keyCode.action.name, true);

            string displayName = key.keyName.GetLocalizedString();
            var binding = action.bindings[bindingIndex];

            if (binding.isPartOfComposite) {
                var entryResult = await LocalizationSettings.StringDatabase.GetTableEntryAsync(key.keyName.TableReference, binding.name).Task;
                if (entryResult.Entry == null)
                    displayName += $" ({binding.name})";
                else
                    displayName += $" ({entryResult.Entry.GetLocalizedString()})";
            }

            Setup(action, bindingIndex, displayName);
        }

        public void OnRebindClick()
        {
            _action.Disable();

            _action.PerformInteractiveRebinding(_bindingIndex)
                .OnComplete(operation =>
                {
                    _keyName.text = _action.GetBindingDisplayString(_bindingIndex);
                    GameInput.Instance.InputActions.FindAction(_action.name)
                        .ApplyBindingOverride(_bindingIndex, _action.bindings[_bindingIndex].overridePath);

                    _action.Enable();
                    operation.Dispose();
                })
                .Start();
        }
    }
}
