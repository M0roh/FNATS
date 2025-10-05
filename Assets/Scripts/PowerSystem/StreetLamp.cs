using UnityEngine;
using VoidspireStudio.FNATS.Utils;

namespace VoidspireStudio.FNATS.PowerSystem
{
    public class StreetLamp : MonoBehaviour, IElectricDevice
    {
        [SerializeField] private GameObject _lightWrapper;

        public bool IsActive { get; private set; }

        public float GetCurrentConsumption => 0.002f;

        private void OnEnable()
        {
            this.RegisterDevice();
        }

        private void OnDisable()
        {
            this.UnregisterDevice();
        }

        public void TurnOff()
        {
            _lightWrapper.SetActive(false);
        }

        public void TurnOn()
        {
            _lightWrapper.SetActive(true);
        }
    }
}
