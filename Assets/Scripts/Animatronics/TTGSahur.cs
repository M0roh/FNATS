using VoidspireStudio.FNATS.Animatronics.Routes;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.Animatronics
{
    public class TTGSahur : AnimatronicAI
    {
        protected override bool BlockCheck() => !OfficeManager.Instance.IsEnabledLight && OfficeManager.Instance.IsPlayerUnderTable;

        protected override void PerformSabotage(SabotageStep _) { }
    }
}