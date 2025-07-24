using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace VoidspireStudio.FNATS.Utils
{
    public static class Extentions
    {
        public static bool HasReachedDestination(this NavMeshAgent agent)
        {
            if (agent == null) return false;

            return !agent.pathPending &&
                   agent.remainingDistance <= agent.stoppingDistance &&
                   (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}
