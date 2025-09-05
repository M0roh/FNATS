namespace VoidspireStudio.FNATS.Interactables
{
    public interface IInteractable
    {
        bool CanInteract { get; }

        void OnInteract();

        void OnInteractEnd();
    }
}