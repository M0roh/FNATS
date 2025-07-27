using UnityEngine;
using UnityEngine.AI;

namespace VoidspireStudio.FNATS.Utils
{
    public static class Extensions
    {
        public static bool HasReachedDestination(this NavMeshAgent agent)
        {
            if (agent == null) return false;

            return !agent.pathPending &&
                   agent.remainingDistance <= agent.stoppingDistance &&
                   (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
        
        public static float Fraction(this float value)
        {
            return value - Mathf.Floor(value);
        }
    }
}
