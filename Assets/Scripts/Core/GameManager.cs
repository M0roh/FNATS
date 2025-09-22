using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using VoidspireStudio.FNATS.Saves;
using VoidspireStudio.FNATS.UI.KeyBindings;

namespace VoidspireStudio.FNATS.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Tips")]
        [SerializeField] private LocalizeStringEvent _tipLocalization;
        [SerializeField] private TMP_Text _tipText;

        [Header("Graphics")]
        [SerializeField] private Volume _globalVolume;

        private int _playerInteractBindingIndex = 0;

        public Coroutine _tipFadeCoroutine;

        private void Awake()
        {
            Instance = this;

            if (_globalVolume.profile.TryGet<Exposure>(out var exp))
                exp.fixedExposure.value = (SaveManager.LastSavedData.graphics.brightness * -8f) + 4f;
            if (_globalVolume.profile.TryGet<MotionBlur>(out var blur))
                blur.active = SaveManager.LastSavedData.graphics.motionBlur;

            var action = GameInput.Instance.InputActions.Player.Interact;
            string currentScheme = GameInput.Instance.CurrentControlScheme;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];

                if (binding.isComposite)
                    continue;

                if (!string.IsNullOrEmpty(binding.groups) &&
                    !binding.groups.Contains(currentScheme))
                    continue;

                _playerInteractBindingIndex = i;
                return;
            }
        }

        public void ShowTip(LocalizedString tipString) 
        {
            _tipText.gameObject.SetActive(true);
            _tipLocalization.StringReference = tipString;
            _tipLocalization.StringReference.Arguments = new object[] { GameInput.Instance.InputActions.Player.Interact.GetBindingDisplayString(_playerInteractBindingIndex) };
            _tipLocalization.RefreshString();
        }

        public void HideTip() => _tipText.gameObject.SetActive(false);
    }
}