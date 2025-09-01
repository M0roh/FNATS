using UnityEngine;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.Interactables
{
    public class FuseToolBox : MonoBehaviour, IInteractable
    {
        public void OnInteract()
        {
            if (!Player.Instance.IsPickedFuse)
                Player.Instance.IsPickedFuse = true;
        }

        public void OnInteractEnd() { }
    }
}
