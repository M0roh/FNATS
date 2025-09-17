using VoidspireStudio.FNATS.Animatronics.Routes;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.Animatronics
{
    public class BrrBrrPatapim : AnimatronicAI
    {
        protected override bool BlockCheck() => true;

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
