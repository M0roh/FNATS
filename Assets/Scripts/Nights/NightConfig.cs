using UnityEngine;

namespace VoidspireStudio.FNATS.Nights
{
    [CreateAssetMenu(menuName = "FNaF/Night Config")]
    public class NightConfig : ScriptableObject
    {
        [SerializeField] private float _animatronicSpeed;
        [SerializeField] private float _energyDrainRate;

        public float AnimatronicSpeed => _animatronicSpeed;
        public float EnergyDrainRate => _energyDrainRate;
    }
}
