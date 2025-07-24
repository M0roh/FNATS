using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoidspireStudio.FNATS.Animatronics.Routes
{
    [CreateAssetMenu(menuName = "FNaF/Animatronic Route")]
    public class AnimatronicRoute : ScriptableObject
    {
        [SerializeField] private string _routeID = Guid.NewGuid().ToString();
        [SerializeField] private string _startNodeId;
        [SerializeField] private List<RouteStep> _steps;

        private Dictionary<string, RouteStep> _cachedMap;

        public Guid RouteID => Guid.Parse(_routeID);

        public void Init()
        {
            _cachedMap = _steps.ToDictionary(s => s.Id);
        }

        public RouteStep GetStepById(string id)
        {
            if (_cachedMap == null) Init();
            return _cachedMap.TryGetValue(id, out var step) ? step : null;
        }

        public RouteStep GetStartStep => GetStepById(_startNodeId);

        public RouteStep GetNearestStep(Vector3 position)
        {
            return _steps
                .OfType<GoToStep>()
                .Where(step => step.Target != null)
                .OrderBy(step => Vector3.SqrMagnitude(step.Target.position - position))
                .FirstOrDefault();
        }
    }
}
