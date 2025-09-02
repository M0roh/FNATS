using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
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
        [SerializeField] private string _waypoint = string.Empty;
        
        public Transform Target => WaypointRegistry.Get(_waypoint).transform;
    }

    [System.Serializable]
    public class RotateStep : RouteStep
    {
        [SerializeField] private string _waypoint = string.Empty;
        
        public Quaternion Target => WaypointRegistry.Get(_waypoint).transform.localRotation;
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