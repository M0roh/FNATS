using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Volume _globalVolume;

        private void Awake()
        {
            if (_globalVolume.profile.TryGet<Exposure>(out var exp))
                exp.fixedExposure.value = (SaveManager.LastSavedData.graphics.brightness * -8f) + 4f;
            if (_globalVolume.profile.TryGet<MotionBlur>(out var blur))
                blur.active = SaveManager.LastSavedData.graphics.motionBlur;
        }
    }
}