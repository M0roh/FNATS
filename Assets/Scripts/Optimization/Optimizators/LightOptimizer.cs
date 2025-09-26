using System;
using UnityEngine;

namespace VoidspireStudio.FNATS.Optimization
{
    [AddComponentMenu("Optimization/LightOptimizer")]
    public class LightOptimizer : OptimizerTarget
    {
        [SerializeField] private float _enableDistance = 20f;
        [SerializeField] private Light _light;

        private void Reset()
        {
            _light = GetComponent<Light>();
        }

        public override void ApplyOptimization(float distance)
        {
            if (_light == null) return;

            bool enable = distance < _enableDistance;
            if (_light.enabled != enable)
                _light.enabled = enable;
        }
    }

}
