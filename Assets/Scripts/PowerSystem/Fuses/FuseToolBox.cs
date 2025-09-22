using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.PowerSystem.Fuses
{
    public class FuseToolBox : MonoBehaviour, IInteractable
    {
        [SerializeField] private LocalizedString _interactTip;
        [SerializeField] private List<GameObject> _fusesInBox = new();

        public bool CanInteract => true;

        public LocalizedString InteractTip => _interactTip;

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
