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

        [SerializeField] private Dictionary<int, NightConfig> _nightConfigs;
        private NightConfig _currentConfig;
        private readonly float _nightTimeEnd = 6.00f;

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

        private void Start()
        {
            NightTime.Reset();
        }

        public void StartNight(int night)
        {
            if (_nightConfigs.TryGetValue(night, out _currentConfig)) {
                StartCoroutine(NightTimer());
                PowerManager.Instance.StartDrain();
            }
            else {
                Debug.LogError("Unknow night!");
            }
        }

        public IEnumerator NightTimer()
        {
            do
            {
                NightTime.Tick();

                yield return new WaitForSeconds(1f);
            } while (NightTime.CurrentTime < _nightTimeEnd);
            
            yield return null;
            
            NightComplete();
        }

        public void NightComplete() 
        {
            ++GameManager.Instance.CurrentNight;
            Debug.Log("Ночь окончена!");
        }
    }
}
