using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoidspireStudio.FNATS.Cameras;

namespace VoidspireStudio.FNATS.Optimization
{
    [AddComponentMenu("Optimization/SceneOptimization")]
    public class SceneOptimization : MonoBehaviour
    {
        private static readonly List<OptimizerTarget> _optimizationTargets = new();

        public static void AddTarget(OptimizerTarget target) => _optimizationTargets.Add(target);
        public static void RemoveTarget(OptimizerTarget target) => _optimizationTargets.Remove(target);

        private void OnEnable()
        {
            StartCoroutine(OptimizeCoroutine());
        }

        private IEnumerator OptimizeCoroutine()
        {
            while (true)
            {
                var pos = (SecurityCamerasManager.Instance != null && SecurityCamerasManager.Instance.IsPlayerOnCameras) ? SecurityCamerasManager.Instance.CurrentCamera.transform.position : Camera.main.transform.position;
                foreach (var target in _optimizationTargets.ToList())
                {
                    var distance = Vector3.Distance(pos, target.transform.position);
                    target.ApplyOptimization(distance);
                }

                yield return null;
                yield return null;
                yield return null;
            }
        }
    }
}
