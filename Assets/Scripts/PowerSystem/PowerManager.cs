using System;
using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.PowerSystem
{
    public class PowerManager : MonoBehaviour
    {
        public static PowerManager Instance { get; private set; }

        [Header("Power Settings")]
        [SerializeField] private float _maxPower = 100f;
        [SerializeField] private float _passiveDrain = 0.03f;
        [SerializeField] private float _lightDrain = 0.05f;

        private float _power;
        private Coroutine _drainCoroutine;

        public event Action<float> OnPowerChanged;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void StartDrain()
        {
            _power = _maxPower;

            _drainCoroutine = StartCoroutine(PowerDrain());
        }

        public void StopDrain()
        {
            _power = 0f;;
    
            if (_drainCoroutine != null)
                StopCoroutine(_drainCoroutine);
        }

        private IEnumerator PowerDrain()
        {
            while (true)
            {
                _power -= _passiveDrain;

                if (GameManager.Instance.IsLightOn)
                    _power -= _lightDrain;

                OnPowerChanged?.Invoke(_power);

                yield return new WaitForSeconds(1f);

                if (_power < 0)
                {
                    Debug.Log("Lose. Power off.");
                    StopDrain();
                }
            }
        }
    }
}
