using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoidspireStudio.FNATS.Player;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.PowerSystem.Fuses
{
    public class FuseToolBox : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<GameObject> _fusesInBox = new();

        public bool CanInteract => true;

        public void OnInteract()
        {
            if (!Player.Player.Instance.IsPickedFuse && _fusesInBox.Count > 0)
            {
                Player.Player.Instance.IsPickedFuse = true;

                var fuse = _fusesInBox.Last();
                Destroy(fuse);
                _fusesInBox.Remove(fuse);

            }
        }

        public void OnInteractEnd() { }
    }
}
