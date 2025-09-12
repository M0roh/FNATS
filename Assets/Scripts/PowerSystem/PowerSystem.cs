using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoidspireStudio.FNATS.Nights;

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

        public bool IsStopped => !IsRunning || IsPaused;

        /// <summary>
        /// Вызывается после изменения электричества
        /// </summary>
        public event PowerChangedHandler OnPowerChanged;
        public delegate void PowerChangedHandler(float currentPower);

        /// <summary>
        /// Вызывается до изменения электричества после расчёта сколько потратиться энергии за этот тик
        /// </summary>
        public event PowerDrainCalculatedHandler OnPowerDrainCalculated;
        public delegate void PowerDrainCalculatedHandler(float currentPower, float drainAmount);

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            NightTime.OnTick += PowerDrain;
            OnPowerChanged?.Invoke(_power);
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
            powerNode.OnBroken += OnBreak;
            powerNode.OnRepair += OnRepaired;
        }

        public void UnregisterPowerNode(IPowerNode powerNode)
        {
            powerNode.OnBroken -= OnBreak;
            powerNode.OnRepair -= OnRepaired;
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

            float drain = _passiveDrain;
            drain += _electricDevices.Where(device => device != null && device.IsActive).Sum(device => device.GetCurrentConsumption);
            drain *= NightManager.Instance.CurrentConfig.EnergyDrainMultiplier;

            OnPowerDrainCalculated(_power, drain);

            _power -= drain;
            OnPowerChanged?.Invoke(_power);

            if (_power < 0)
            {
                Debug.Log("Lose. Power off.");
                StopConsumption(true);
            }
        }

        private void OnRepaired()
        {
            if (_powerNodes.All(powerNode => powerNode.IsActive))
            {
                Resume();
                UpdateDevicesState(true);
            }
        }

        private void OnBreak()
        {
            Pause();
            UpdateDevicesState(false);
        }

        private void UpdateDevicesState(bool active)
        {
            _electricDevices.RemoveAll(d => d == null);

            foreach (var device in _electricDevices)
            {
                if (active) device.TurnOn();
                else device.TurnOff();
            }
        }

    }
}
