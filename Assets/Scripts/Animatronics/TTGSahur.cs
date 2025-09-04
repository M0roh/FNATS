using VoidspireStudio.FNATS.Animatronics.Routes;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.Animatronics
{
    public class TTGSahur : AnimatronicAI
    {
        protected override void OfficeAtack()
        {
        }

        protected override void PerformSabotage(SabotageStep step)
        {
            if (step.SabotageType == SabotageType.BreakGenerator)
            {
                _animator.SetTrigger(ATTACK);
                Generator.Instance.TurnOff();
            }
        }
    }
}