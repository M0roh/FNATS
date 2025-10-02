using System;

namespace VoidspireStudio.FNATS.Interactables
{
    public interface IMachineEvents
    {
        event Action<bool> OnActiveChange;
        event Action OnBroken;
    }

}
