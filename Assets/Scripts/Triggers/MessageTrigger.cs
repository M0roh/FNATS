using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.Triggers
{
    public class MessageTrigger : ColliderTrigger
    {
        [SerializeField] private LocalizedString _messageOnEnter;
        [SerializeField] private LocalizedString _messageOnStay;
        [SerializeField] private LocalizedString _messageOnExit;

        private void Awake()
        {
            _onEnter.AddListener(() => SendMessage(_messageOnEnter));
            _onStay.AddListener(() => SendMessage(_messageOnStay));
            _onExit.AddListener(() => SendMessage(_messageOnExit));
        }

        public void SendMessage(LocalizedString message)
        {
            if (message.IsEmpty) return;

            GameManager.Instance.SendMessage(message);
        }
    }
}
