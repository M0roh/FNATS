using System.Collections.Generic;
using UnityEngine;

namespace VoidspireStudio.FNATS.Animatronics.Routes
{
    public static class WaypointRegistry
    {
        private static readonly Dictionary<string, Waypoint> _map = new();

        public static void Register(string id, Waypoint wp)
        {
            if (string.IsNullOrEmpty(id) || wp == null) return;

            if (_map.TryGetValue(id, out var existing))
            {
                if (existing != wp)
                    Debug.LogWarning($"[WaypointRegistry] Duplicate id '{id}' detected. Existing: {existing.name}, New: {wp.name}");
                return;
            }

            _map[id] = wp;
        }

        public static void Unregister(string id, Waypoint wp)
        {
            if (string.IsNullOrEmpty(id) || wp == null) return;
            if (_map.TryGetValue(id, out var existing) && existing == wp)
                _map.Remove(id);
        }

        public static Waypoint Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (!_map.TryGetValue(id, out var wp))
                Debug.LogWarning("[WaypointRegistry] Waypoint does not found!");
            return wp;
        }
    }
}
