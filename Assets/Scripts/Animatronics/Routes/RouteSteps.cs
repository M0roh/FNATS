using System.Collections.Generic;
using UnityEngine;

namespace VoidspireStudio.FNATS.Animatronics.Routes
{
    public enum SabotageType
    {
        BreakGenerator,
        Short—ircuit
    }

    [System.Serializable]
    public abstract class RouteStep
    {
        [SerializeField] private string _id = string.Empty;
        [SerializeField] private List<string> _nextStepsIds = new();

        public string Id => _id;
        public IReadOnlyList<string> NextStepsIds => _nextStepsIds;
        public bool HasNextSteps => _nextStepsIds.Count > 0;
    }

    [System.Serializable]
    public class GoToStep : RouteStep
    {
        [SerializeField] private Transform _target;

        public Transform Target => _target;
    }

    [System.Serializable]
    public class WaitStep : RouteStep
    {
        [SerializeField] private float _waitTime;

        public float WaitTime => _waitTime;
    }

    [System.Serializable]
    public class SabotageStep : RouteStep
    {
        [SerializeField] private SabotageType _sabotageType;

        public SabotageType SabotageType => _sabotageType;
    }

    [System.Serializable]
    public class AttackStep : RouteStep { }

}