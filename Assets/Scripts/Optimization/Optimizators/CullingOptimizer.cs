using UnityEngine;

namespace VoidspireStudio.FNATS.Optimization
{
    [AddComponentMenu("Optimization/CullingOptimizer")]
    public class CullingOptimizer : OptimizerTarget
    {
        [SerializeField] private float _cullingDistance = 30f;

        public override void ApplyOptimization(float distance)
        {
            bool active = distance < _cullingDistance;
            if (gameObject.activeInHierarchy != active)
                gameObject.SetActive(active);
        }
    }
}
