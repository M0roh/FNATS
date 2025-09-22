using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.PowerSystem.Fuses
{
    public class FuseBox : MonoBehaviour, IPowerNode
    {
        [SerializeField] private LocalizedString _interactTip;

        [Header("Предохранители")]
        [SerializeField] private List<Fuse> _fuses;

        [Header("Свет")]
        [SerializeField] private List<Light> _lights;
        [SerializeField] private float _fadeTime = 1f;
        [SerializeField] private float _maxIntensity;
        private List<int> _availableIndices = new();

        public bool IsActive { get; private set; }
        public bool IsRepaired => _fuses.All(fuse => fuse.IsActive);

        public LocalizedString InteractTip => _interactTip;

        public event Action OnBroken;
        public event Action OnRepair;

        private void Start()
        {
            this.RegisterNode();

            foreach (var light in _lights)
                light.intensity = 0f;

            IgniteNextLight();
        }

        private void OnEnable()
        {
            PowerSystem.Instance.OnPowerDrainCalculated += BreakChance;
        }

        private void OnDisable()
        {
            PowerSystem.Instance.OnPowerDrainCalculated -= BreakChance;
        }

        private void OnDestroy()
        {
            this.UnregisterNode();
        }

        private void IgniteNextLight()
        {
            if (_availableIndices.Count == 0)
                for (int i = 0; i < _lights.Count; i++)
                    _availableIndices.Add(i);

            int randomIndex = _availableIndices[UnityEngine.Random.Range(0, _availableIndices.Count)];
            _availableIndices.Remove(randomIndex);

            StartCoroutine(FadeLight(_lights[randomIndex]));
        }

        private IEnumerator FadeLight(Light light)
        {
            float timer = 0f;

            while (timer < _fadeTime)
            {
                light.intensity = Mathf.Lerp(0f, _maxIntensity, timer / _fadeTime);
                timer += Time.deltaTime;
                yield return null;
            }

            light.intensity = _maxIntensity;

            IgniteNextLight();

            timer = 0f;

            while (timer < _fadeTime)
            {
                light.intensity = Mathf.Lerp(_maxIntensity, 0f, timer / _fadeTime);
                timer += Time.deltaTime;
                yield return null;
            }

            light.intensity = 0f;
        }

        public void RepairCheck()
        {
            if (IsRepaired && !IsActive)
            {
                IsActive = true;
                OnRepair?.Invoke();
            }
        }

        public void Off()
        {
            OnBroken?.Invoke();
        }

        private void BreakChance(float currentPower, float drainAmount)
        {
            if (drainAmount <= 0f) return;

            float baseChance = 0.05f;

            float loadFactor = Mathf.Clamp01(drainAmount / Mathf.Max(currentPower, 1f));
            float breakChance = baseChance * Mathf.Pow(loadFactor, 0.8f);

            if (UnityEngine.Random.value < breakChance)
            {
                Off();
                _fuses[UnityEngine.Random.Range(0, _fuses.Count)].Break();
            }
        }
    }
}