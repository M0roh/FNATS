using UnityEngine;

namespace VoidspireStudio.FNATS.Optimization
{
    public abstract class OptimizerTarget : MonoBehaviour
    {
        public abstract void ApplyOptimization(float distance);

        private void OnEnable()
        {
            SceneOptimization.AddTarget(this);
        }

        private void OnDisable()
        {
            SceneOptimization.RemoveTarget(this);
        }
    }
}
