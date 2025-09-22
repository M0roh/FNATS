using UnityEngine.Localization;

namespace VoidspireStudio.FNATS.Interactables
{
    public interface IInteractable
    {
        LocalizedString InteractTip { get; }

        bool CanInteract { get; }

        void OnInteract();

        void OnInteractEnd();
    }
}