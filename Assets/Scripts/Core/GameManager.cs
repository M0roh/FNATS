using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Messages")]
        [SerializeField] private TMP_Text _messagesText;
        [SerializeField] private LocalizeStringEvent _messagesLocalization;
        [SerializeField] private float _fadeDelay = 1f;
        [SerializeField] private float _fadeDuration = 4f;

        [Header("Tips")]
        [SerializeField] private LocalizeStringEvent _tipLocalization;
        [SerializeField] private TMP_Text _tipText;

        [Header("Graphics")]
        [SerializeField] private Volume _globalVolume;

        public Volume GlobalVolume => _globalVolume;

        private int _playerInteractBindingIndex = 0;

        public CancellationTokenSource _messageFadeToken;

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

        private void Start()
        {
            GameInput.Instance.InputActions.Player.Enable();

        }

        public float GetSentivity()
        {
            float sentivity = SaveManager.LastSavedData.gameplay.mouseSensitivity;

            if (GameInput.Instance.CurrentControlScheme.Equals("Gamepad", System.StringComparison.OrdinalIgnoreCase)
                || GameInput.Instance.CurrentControlScheme.Equals("Joystick", System.StringComparison.OrdinalIgnoreCase))
                sentivity *= 10;

            return sentivity;
        }

        public void ShowTip(LocalizedString tipString) 
        {
            _tipText.gameObject.SetActive(true);
            _tipLocalization.StringReference = tipString;
            _tipLocalization.StringReference.Arguments = new object[] { GameInput.Instance.InputActions.Player.Interact.GetBindingDisplayString(_playerInteractBindingIndex) };
            _tipLocalization.RefreshString();
        }

        public void HideTip() => _tipText.gameObject.SetActive(false);

        public async UniTask SendMessage(LocalizedString message)
        {
            _messagesText.gameObject.SetActive(true);
            _messagesLocalization.StringReference = message;
            _messagesLocalization.RefreshString();

            if (_messageFadeToken?.Token.CanBeCanceled ?? false)
                _messageFadeToken.Cancel();

            _messageFadeToken = CancellationTokenSource.CreateLinkedTokenSource(new(), this.GetCancellationTokenOnDestroy());
            try {
                await MessageFade();
            }
            finally
            {
                _messageFadeToken?.Dispose();
                _messageFadeToken = null;
            }
        }

        public async UniTask MessageFade()
        {
            Color baseColor = _messagesText.color;
            baseColor.a = 1f;
            _messagesText.color = baseColor;

            await UniTask.Delay(Mathf.RoundToInt(_fadeDelay * 1000f), cancellationToken: _messageFadeToken.Token);

            float timer = _fadeDuration;
            while (baseColor.a > 0f)
            {
                timer -= Time.deltaTime;
                baseColor.a = Mathf.Lerp(baseColor.a, 0f, Time.deltaTime / timer);
                _messagesText.color = baseColor;
                await UniTask.Yield(cancellationToken: _messageFadeToken.Token);
            }

            _messagesText.gameObject.SetActive(false);
        }
    }
}