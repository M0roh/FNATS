using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoidspireStudio.FNATS.Nights
{
    [CreateAssetMenu(menuName = "FNaF/Night Config")]
    public class NightConfig : ScriptableObject
    {
        [SerializeField] private float _animatronicSpeedMultiplier;
        [SerializeField] private float _energyDrainRateMultiplier;

        [SerializeField] private Dictionary<string, (int hour, int minute)> _animatronicActivity = new();

        public float AnimatronicSpeedMultiplier => _animatronicSpeedMultiplier;
        public float EnergyDrainMultiplier => _energyDrainRateMultiplier;

        public Dictionary<string, (int hour, int minute)> AnimatronicActivity => new(_animatronicActivity);
    }
}
