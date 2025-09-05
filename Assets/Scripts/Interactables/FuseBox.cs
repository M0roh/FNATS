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
        [Header("Предохранители")]
        [SerializeField] private GameObject _fuseObject1;
        [SerializeField] private GameObject _fuseObject2;

        public bool IsActive { get; private set; }

        public bool CanInteract => !IsActive;

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
            if (Player.Instance.IsPickedFuse)
            {
                Player.Instance.IsPickedFuse = false;
                IsActive = true;
                OnRepair?.Invoke();

                if (_fuseObject1.activeSelf)
                {
                    _fuseObject2.SetActive(true);
                }
                else
                {
                    _fuseObject1.SetActive(true);
                }
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

                if (UnityEngine.Random.Range(1, 3) == 1)
                {
                    _fuseObject1.SetActive(false);
                }
                else 
                { 
                    _fuseObject2.SetActive(false);
                }
            }
        }
    }
}