using TMPro;
using UnityEngine;
using VoidspireStudio.FNATS.Nights;

namespace VoidspireStudio.FNATS.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class NightTimeUI : MonoBehaviour
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            NightTime.OnTick += OnTick_TimeUpdate;
        }

        private void OnTick_TimeUpdate(GameTime time)
        {
            _text.text = time.GetFormattedTime();
        }
    }
}
