using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.PowerSystem;

namespace VoidspireStudio.FNATS.Core
{
    public class NightManager : MonoBehaviour
    {
        public static NightManager Instance { get; private set; }

        private float _nightDuration = 6 * 60f;

        private float _timer;

        public int GetHour => Mathf.FloorToInt(_timer / 60);
        public int GetMinutes => Mathf.RoundToInt(((_timer / 60) - GetHour) * 60);

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

        public void StartNight()
        {
            _timer = 0;

            StartCoroutine(NightTimer());
            PowerManager.Instance.StartDrain();
        }

        public IEnumerator NightTimer()
        {
            _timer += 1f;

            yield return new WaitForSeconds(1f);

            if (_timer >= _nightDuration)
                NightComplete();
        }

        //TODO: Окончание ночи. Переброс на экран 
        public void NightComplete() 
        {
            ++GameManager.Instance.CurrentNight;
            Debug.Log("Ночь окончена!");
        }
    }
}
