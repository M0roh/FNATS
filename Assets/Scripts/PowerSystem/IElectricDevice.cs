namespace VoidspireStudio.FNATS.PowerSystem { 
    public interface IElectricDevice
    {
        public float GetCurrentConsumption { get; }
        public float TurnOn();
        public float TurnOff();
    }
}
