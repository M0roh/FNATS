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
        [SerializeField] private float _passiveDrain = 0.05f;
        [SerializeField] private float _lightDrain = 0.1f;

        private float _power;

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

            StartCoroutine(PowerDrain());
        }

        private IEnumerator PowerDrain()
        {
            _power -= _passiveDrain;

            if (GameManager.Instance.IsLightOn)
                _power -= _lightDrain;

            yield return new WaitForSeconds(1f);

            if (_power < 0)
            {
                Debug.Log("Lose. Power off.");
            }
        }
    }
}
