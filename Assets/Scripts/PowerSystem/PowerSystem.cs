using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoidspireStudio.FNATS.Nights;
using static UnityEngine.EventSystems.EventTrigger;

namespace VoidspireStudio.FNATS.PowerSystem
{
    public class PowerSystem : MonoBehaviour
    {
        public static PowerSystem Instance { get; private set; }

        [Header("Power Settings")]
        [SerializeField] private float _maxPower = 100f;
        [SerializeField] private float _passiveDrain = 0.03f;

        private readonly List<IElectricDevice> _electricDevices = new();
        private readonly List<IPowerNode> _powerNodes = new();

        private float _power;
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

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

        private void OnEnable()
        {
            NightTime.OnTick += PowerDrain;
        }

        private void OnDisable()
        {
            NightTime.OnTick -= PowerDrain;
        }

        public void RegisterDevice(IElectricDevice device)
        {
            _electricDevices.Add(device);
        }

        public void UnregisterDevice(IElectricDevice device)
        {
            _electricDevices.Remove(device);
        }

        public void RegisterPowerNode(IPowerNode powerNode)
        {
            _powerNodes.Add(powerNode);
            powerNode.OnBroken += Pause;
            powerNode.OnRepair += CheckBreak;
        }

        public void UnregisterPowerNode(IPowerNode powerNode)
        {
            powerNode.OnBroken -= Pause;
            powerNode.OnRepair -= CheckBreak;
            _powerNodes.Remove(powerNode);
        }

        public bool IsDeviceRegistred(IElectricDevice device) => _electricDevices.Contains(device);

        public void StartConsumption()
        {
            _power = _maxPower;
            IsRunning = true;
            IsPaused = false;
            OnPowerChanged?.Invoke(_power);
        }

        public void StopConsumption(bool resetToZero = false)
        {
            IsRunning = false;
            IsPaused = false;
            if (resetToZero) _power = 0f;
            OnPowerChanged?.Invoke(_power);
        }

        public void Pause() { IsPaused = true; }
        public void Resume() { if (IsRunning) IsPaused = false; }

        private void PowerDrain(GameTime _)
        {
            if (!IsRunning || IsPaused) return;

            _power -= _passiveDrain;

            foreach (var device in _electricDevices)
                _power -= device.GetCurrentConsumption;

            OnPowerChanged?.Invoke(_power);

            if (_power < 0)
            {
                Debug.Log("Lose. Power off.");
                StopConsumption(true);
            }
        }

        private void CheckBreak()
        {
            if (_powerNodes.All(powerNode => powerNode.IsActive))
                IsPaused = false;
            else
                IsPaused = true;
        }
    }
}
