using System;
using UnityEngine;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.Interactables;
using VoidspireStudio.FNATS.PowerSystem;
using VoidspireStudio.FNATS.Utils;

namespace Assets.Scripts.Interactables
{
    public class FuseBox : MonoBehaviour, IInteractable, IPowerNode
    {
        public bool IsActive { get; private set; }

        public event Action OnBroken;
        public event Action OnRepair;

        private void Start()
        {
            this.RegisterNode();
        }

        private void OnEnable()
        {
            PowerSystem.Instance.OnPowerDrainCalculated += BreakChance;
        }

        private void OnDisable()
        {
            PowerSystem.Instance.OnPowerDrainCalculated -= BreakChance;
        }

        private void OnDestroy()
        {
            this.UnregisterNode();
        }

        public void OnInteract()
        {
            // TODO: Предохранитель визуально ставить на место
            if (Player.Instance.IsPickupedFuse)
            {
                IsActive = true;
                OnRepair?.Invoke();
            }
        }

        public void OnInteractEnd() { }

        private void BreakChance(float currentPower, float drainAmount)
        {
            if (drainAmount <= 0f) return;

            float baseChance = 0.01f;

            float loadFactor = Mathf.Clamp01(drainAmount / Mathf.Max(currentPower, 1f));
            float breakChance = baseChance * loadFactor;

            if (UnityEngine.Random.value < breakChance)
            {
                IsActive = false;
                OnBroken?.Invoke();
            }
        }
    }
}