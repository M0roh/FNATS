using UnityEngine;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.Core
{
    public class OfficeManager : MonoBehaviour
    {
        [SerializeField] private LightSwitcher _officeLight;
        [SerializeField] private Door _officeDoor;

        public bool IsPlayerInOffice { get; private set; } = false;
        public bool IsPlayerUnderTable { get; private set; } = false;
        public bool IsPlayerNearOffice { get; private set; } = false;
        public bool IsOpenedDoor => _officeDoor.IsOpen;
        public bool IsEnabledLight => _officeLight.IsActive;

        public void PlayerHideUnderTable() => IsPlayerUnderTable = true;
        public void PlayerUnhideUnderTable() => IsPlayerUnderTable = false;

        public void PlayerEnterOffice() => IsPlayerInOffice = true;
        public void PlayerExitOffice() => IsPlayerInOffice = false;

        public void PlayerNearOffice() => IsPlayerNearOffice = true;
        public void PlayerNotNearOffice() => IsPlayerNearOffice = false;
    }
}
