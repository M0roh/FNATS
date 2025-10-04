using UnityEngine;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.PowerSystem;

namespace VoidspireStudio.FNATS.Sounds.Game
{
    public class EventsSounds : MonoBehaviour
    {
        [Header("Power Audio")]
        private AudioClip _onPowerBreak;
        private AudioClip _onPowerOff;

        [Header("Night Audio")]
        private AudioClip _onOfficeBreak;
        private AudioClip _onNightEnd;

        private void OnEnable()
        {
            PowerSystem.PowerSystem.Instance.OnPowerBreak += OnPowerBreak;
            PowerSystem.PowerSystem.Instance.OnPowerLeft += OnPowerLeft;

            NightTime.OnTick += NightTime_OnTick;
        }

        private void NightTime_OnTick(GameTime time)
        {
            if (time.IsTime(6, 00))
                AudioManager.Instance.PlaySound2D(_onNightEnd, AudioManager.AudioType.Ambient);
        }

        private void OnPowerLeft()
        {
            AudioManager.Instance.PlaySound2D(_onPowerOff, AudioManager.AudioType.Ambient);
        }

        private void OnPowerBreak()
        {
            AudioManager.Instance.PlaySound2D(_onPowerBreak, AudioManager.AudioType.Ambient);
        }
    }
}
