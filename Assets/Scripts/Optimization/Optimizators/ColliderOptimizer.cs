using UnityEngine;

namespace VoidspireStudio.FNATS.Optimization
{
    [AddComponentMenu("Optimization/ColliderOptimizer")]
    public class ColliderOptimizer : OptimizerTarget
    {
        [SerializeField] private float _enableDistance = 50f;
        [SerializeField] private Collider _collider;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
        }

        public override void ApplyOptimization(float distance)
        {
            if (_collider == null) return;

            bool enable = distance < _enableDistance;
            if (_collider.enabled != enable)
                _collider.enabled = enable;
        }
    }
}