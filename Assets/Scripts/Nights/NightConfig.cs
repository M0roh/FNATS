using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoidspireStudio.FNATS.Nights
{
    [System.Serializable]
    public class ActivityTime
    {
        public int hour;
        public int minute;
    }

    [CreateAssetMenu(menuName = "FNaF/Night Config")]
    public class NightConfig : SerializedScriptableObject
    {
        [SerializeField] private float _animatronicSpeedMultiplier;
        [SerializeField] private float _energyDrainRateMultiplier;

        [OdinSerialize] private Dictionary<string, ActivityTime> _animatronicActivity = new();

        public float AnimatronicSpeedMultiplier => _animatronicSpeedMultiplier;
        public float EnergyDrainMultiplier => _energyDrainRateMultiplier;

        public Dictionary<string, ActivityTime> AnimatronicActivity => new(_animatronicActivity);
    }
}
