using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using VoidspireStudio.FNATS.PowerSystem;

namespace VoidspireStudio.FNATS.Utils
{
    public static class Extensions
    {
        public static async UniTask Rotate(this Transform transform, Quaternion targetAngle, float speed, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested && transform != null && Quaternion.Angle(transform.localRotation, targetAngle) > 0.01f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetAngle, speed * Time.deltaTime);

                await UniTask.Yield(cancellationToken: cancellationToken).SuppressCancellationThrow();
            }

            if (transform != null)
                transform.localRotation = targetAngle;
        }

        public static bool HasReachedDestination(this NavMeshAgent agent)
        {
            if (agent == null) return false;

            return !agent.pathPending &&
                   agent.remainingDistance <= agent.stoppingDistance &&
                   (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }

        public static bool IsValidPath(this NavMeshAgent agent)
        {
            if (agent == null) return false;

            return agent.hasPath &&
                !agent.pathPending &&
                agent.pathStatus == NavMeshPathStatus.PathComplete;
        }
        
        public static float Fraction(this float value)
        {
            return value - Mathf.Floor(value);
        }

        public static void RegisterDevice(this IElectricDevice device) => PowerSystem.PowerSystem.Instance.RegisterDevice(device);
        public static void UnregisterDevice(this IElectricDevice device) => PowerSystem.PowerSystem.Instance.UnregisterDevice(device);

        public static void RegisterNode(this IPowerNode node) => PowerSystem.PowerSystem.Instance.RegisterPowerNode(node);
        public static void UnregisterNode(this IPowerNode node) => PowerSystem.PowerSystem.Instance.UnregisterPowerNode(node);

        public static bool IsRegisterted(this IElectricDevice device) => PowerSystem.PowerSystem.Instance.IsDeviceRegistred(device);
    }
}
