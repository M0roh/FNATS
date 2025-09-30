using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VoidspireStudio.FNATS.Sounds.UI
{
    public class UISounds : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        [SerializeField] private AudioClip _hoverSound;
        [SerializeField] private AudioClip _clickSound;

        private void Awake()
        {
            if (TryGetComponent<Button>(out var btn))
                btn.onClick.AddListener(PlayClickSound);

            if (TryGetComponent<Toggle>(out var toggle))
                toggle.onValueChanged.AddListener(_ => PlayClickSound());

            if (TryGetComponent<Slider>(out var slider))
                slider.onValueChanged.AddListener(_ => PlayClickSound());

            if (TryGetComponent<TMP_Dropdown>(out var drop))
                drop.onValueChanged.AddListener(_ => PlayClickSound());
        }

        public void PlayClickSound()
        {
            AudioManager.Instance.PlaySound2D(_clickSound, AudioManager.AudioType.SFX);
        }

        public void OnSelect(BaseEventData _)
        {
            AudioManager.Instance.PlaySound2D(_hoverSound, AudioManager.AudioType.SFX);
        }

        public void OnPointerEnter(PointerEventData _)
        {
            AudioManager.Instance.PlaySound2D(_hoverSound, AudioManager.AudioType.SFX);
        }
    }
}