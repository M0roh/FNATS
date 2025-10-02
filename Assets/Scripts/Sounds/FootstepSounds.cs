using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoidspireStudio.FNATS.Sounds
{
    [CreateAssetMenu(menuName = "Sounds/Footsteps")]
    public class FootstepSounds : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<string, AudioClip> _stepSounds = new();
        public AudioClip GetByFloorMaterial(string materialName)
        {
            if (!_stepSounds.TryGetValue(materialName, out AudioClip clip))
                clip = _stepSounds.First().Value;

            return clip;
        }
    }

}