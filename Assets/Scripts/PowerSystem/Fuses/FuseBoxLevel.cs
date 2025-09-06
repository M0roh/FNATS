using System;
using System.Collections; 
using UnityEngine;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.PowerSystem.Fuses
{
    public class FuseBoxLevel : MonoBehaviour, IInteractable
    {
        [Header("Коробка")]
        [SerializeField] private FuseBox _fuseBox;

        [Header("Рычаг")]
        [SerializeField] private Quaternion _levelOnRotation;
        [SerializeField] private Quaternion _levelOffRotation;
        [SerializeField] private float _levelRotationSpeed;

        private Coroutine _rotateCoroutine;

        public bool IsOn { get; private set; }

        public bool CanInteract => !_fuseBox.IsRepaired;

        private void OnEnable()
        {
            _fuseBox.OnBroken += ToOff;
        }

        private void ToOff()
        {
            _rotateCoroutine = StartCoroutine(Rotate(_levelOffRotation, () => _fuseBox.Off()));
        }

        private void ToOn()
        {
            if (_fuseBox.IsRepaired)
                _rotateCoroutine = StartCoroutine(Rotate(_levelOnRotation, () => _fuseBox.RepairCheck()));
            else
                _rotateCoroutine = StartCoroutine(Rotate(_levelOnRotation, ToOff));
        }

        public void OnInteract()
        {
            StopCoroutine(_rotateCoroutine);

            IsOn = !IsOn;

            if (IsOn)
                ToOn();
            else
                ToOff();
        }

        public void OnInteractEnd() { }

        public IEnumerator Rotate(Quaternion targetAngle, Action onComplete = null)
        {
            while (Quaternion.Angle(transform.localRotation, targetAngle) > 0.01f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetAngle, _levelRotationSpeed * Time.deltaTime);

                yield return null;
            }
            transform.localRotation = targetAngle;

            onComplete?.Invoke();
        }
    }
}
