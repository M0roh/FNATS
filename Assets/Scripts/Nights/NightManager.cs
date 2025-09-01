using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace VoidspireStudio.FNATS.Nights
{
    public class NightManager : SerializedMonoBehaviour
    {
        public static NightManager Instance { get; private set; }

        [OdinSerialize] private Dictionary<int, NightConfig> _nightConfigs = new();
        private NightConfig _currentConfig;

        public int CurrentNight { get; set; } = 1;

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
            if (!time.IsTime(0, 0)) return;

            if (_nightConfigs.TryGetValue(CurrentNight, out _currentConfig)) {
                PowerSystem.PowerSystem.Instance.StartConsumption();
            }
            else {
                Debug.LogError("Unknow night!");
            }
        }

        public void NightComplete(GameTime time)
        {
            if (!time.IsTime(6, 0)) return;

            ++CurrentNight;
            Debug.Log("Ночь окончена!");
        }
    }
}
