using VoidspireStudio.FNATS.Animatronics.Routes;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.Animatronics
{
    public class TTTSahur : AnimatronicAI
    {
        protected override bool BlockCheck() => !OfficeManager.Instance.IsOpenedDoor;

        protected override void PerformSabotage(SabotageStep _) { }
    }
}
