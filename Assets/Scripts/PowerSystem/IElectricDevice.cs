namespace VoidspireStudio.FNATS.PowerSystem { 
    public interface IElectricDevice
    {
        public bool IsActive { get; }
        public float GetCurrentConsumption { get; }
        public void TurnOn();
        public void TurnOff();
    }
}
