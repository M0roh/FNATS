using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Interactables;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.PowerSystem.Fuses
{
    public class FuseBoxLevel : MonoBehaviour, IInteractable
    {
        [SerializeField] private LocalizedString _interactTip;

        [Header("Коробка")]
        [SerializeField] private FuseBox _fuseBox;

        [Header("Рычаг")]
        [SerializeField] private Quaternion _levelOnRotation;
        [SerializeField] private Quaternion _levelOffRotation;
        [SerializeField] private float _levelRotationSpeed;

        private CancellationTokenSource _rotateCoroutineToken;

        public bool IsOn { get; private set; } = true;

        public bool CanInteract => _fuseBox.IsRepaired && !_fuseBox.IsActive;

        public LocalizedString InteractTip => _interactTip;

        private void OnEnable()
        {
            _fuseBox.OnBroken += ToOffWrapper;
            ToOn().Forget();
        }

        private void OnDisable()
        {
            _fuseBox.OnBroken -= ToOffWrapper;
        }

        private void ToOffWrapper() => ToOff().Forget();

        private async UniTask ToOff()
        {
            if (_rotateCoroutineToken?.Token.CanBeCanceled ?? false)
                _rotateCoroutineToken.Cancel();

            _rotateCoroutineToken = CancellationTokenSource.CreateLinkedTokenSource(new(), this.GetCancellationTokenOnDestroy());
            try
            {
                await transform.Rotate(_levelOffRotation, _levelRotationSpeed, _rotateCoroutineToken.Token);
            }
            finally
            {
                _rotateCoroutineToken.Dispose();
                _rotateCoroutineToken = null;

                _fuseBox.Off();
            }
        }

        private async UniTask ToOn()
        {
            if (_rotateCoroutineToken?.Token.CanBeCanceled ?? false)
                _rotateCoroutineToken.Cancel();

            _rotateCoroutineToken = CancellationTokenSource.CreateLinkedTokenSource(new(), this.GetCancellationTokenOnDestroy());
            try
            {
                await transform.Rotate(_levelOnRotation, _levelRotationSpeed, _rotateCoroutineToken.Token);
            }
            finally
            {
                _rotateCoroutineToken?.Dispose();
                _rotateCoroutineToken = null;

                if (_fuseBox.IsRepaired)
                    _fuseBox.RepairCheck();
                else
                    await ToOff();
            }
        }

        public void OnInteract()
        {
            if (_rotateCoroutineToken?.Token.CanBeCanceled ?? false)
                _rotateCoroutineToken.Cancel();

            IsOn = !IsOn;

            if (IsOn)
                ToOn().Forget();
            else
                ToOff().Forget();
        }

        public void OnInteractEnd() { }
    }
}
