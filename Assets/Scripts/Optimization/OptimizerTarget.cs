using UnityEngine;

namespace VoidspireStudio.FNATS.Optimization
{
    public abstract class OptimizerTarget : MonoBehaviour
    {
        public abstract void ApplyOptimization(float distance);

        private void Start()
        {
            SceneOptimization.AddTarget(this);
        }

        private void OnDestroy()
        {
            SceneOptimization.RemoveTarget(this);
        }
    }
}
