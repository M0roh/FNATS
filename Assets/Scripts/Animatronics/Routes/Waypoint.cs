using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace VoidspireStudio.FNATS.Animatronics.Routes
{
    public class Waypoint : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        [SerializeField]
        private string _id = "";

        public string Id => _id;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
                RegenerateID();
        }

        private void OnEnable()
        {
            WaypointRegistry.Register(_id, this);
        }

        private void OnDisable()
        {
            WaypointRegistry.Unregister(_id, this);
        }

        [Button]
        private void RegenerateID() => _id = Guid.NewGuid().ToString("N");

        [Button]
        private void CopyIdToClipboard()
        {
            GUIUtility.systemCopyBuffer = _id.ToString();
        }
    }
}