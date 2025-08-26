using System;

namespace VoidspireStudio.FNATS.PowerSystem
{
    public interface IPowerNode
    {
        public bool IsActive { get; }

        event Action OnBroken;
        event Action OnRepair;
    }
}
