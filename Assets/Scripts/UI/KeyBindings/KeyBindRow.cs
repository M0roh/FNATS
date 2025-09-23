using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

            _inputName.text = !string.IsNullOrEmpty(displayNameOverride)
                ? displayNameOverride
                : binding.isPartOfComposite ? binding.name : action.name;

            _keyName.text = action.GetBindingDisplayString(bindingIndex);
        }

        public void Setup(KeyRebinding.Key key, int bindingIndex)
        {
            Setup(key.keyCode.action, bindingIndex, key.keyName.GetLocalizedString());
        }

        public void OnRebindClick()
        {
            _action.Disable();

            _action.PerformInteractiveRebinding(_bindingIndex)
                .OnComplete(operation =>
                {
                    _keyName.text = _action.GetBindingDisplayString(_bindingIndex);
                    _action.Enable();
                    operation.Dispose();
                })
                .Start();
        }
    }
}
