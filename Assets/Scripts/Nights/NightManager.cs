using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoidspireStudio.FNATS.Core;
using VoidspireStudio.FNATS.PowerSystem;

namespace VoidspireStudio.FNATS.Nights
{
    public class NightManager : MonoBehaviour
    {
        public static NightManager Instance { get; private set; }

        [SerializeReference] private Dictionary<int, NightConfig> _nightConfigs = new();
        private NightConfig _currentConfig;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            NightTime.OnTick += StartNight;
            NightTime.OnTick += NightComplete;
        }

        private void OnDisable()
        {
            NightTime.OnTick -= StartNight;
            NightTime.OnTick -= NightComplete;
        }

        public void StartNight(GameTime time)
        {
            if (time.Time != 0) return;

            if (_nightConfigs.TryGetValue(GameManager.Instance.CurrentNight, out _currentConfig)) {
                PowerManager.Instance.StartDrain();
            }
            else {
                Debug.LogError("Unknow night!");
            }
        }

        public void NightComplete(GameTime time)
        {
            if (time.Time != 6.0) return;
            
            ++GameManager.Instance.CurrentNight;
            Debug.Log("Ночь окончена!");
        }
    }
}
