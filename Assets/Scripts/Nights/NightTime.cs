using System;
using System.Collections;
using UnityEngine;

namespace VoidspireStudio.FNATS.Nights
{
    public class NightTime : MonoBehaviour
    {
        public static GameTime CurrentTime { get; private set; }
        public static float TimeScale { get; private set; } = 60f;

        public static event Action<GameTime> OnTick;

        private void OnEnable()
        {
            Reset();
            StartCoroutine(NightTimer());
        }

        public static void Reset(float startHour = 23f, float secondsPerGameHour = 60f)
        {
            CurrentTime = new(startHour);
            TimeScale = 1f / secondsPerGameHour;
        }

        public IEnumerator NightTimer()
        {
            do
            {
                CurrentTime.AddTime(0.01f);

                OnTick?.Invoke(CurrentTime);

                yield return new WaitForSeconds(0.01f / TimeScale);
            } while (true);
        }
    }


    public class GameTime
    {
        private float _time = 0f;

        public float Time => MathF.Round(_time, 2);

        public GameTime(float startTime = 0f)
        {
            _time = NormalizeHour(startTime);
        }

        public void AddTime(float time)
        {
            _time = NormalizeHour(_time + time);
        }

        public void RemoveTime(float time)
        {
            _time = NormalizeHour(_time - time);
        }

        public string GetFormattedTime()
        {
            int hour = Mathf.FloorToInt(_time);
            int minute = Mathf.FloorToInt((_time - hour) * 60f);
            return $"{hour:00}:{minute:00}";
        }

        private float NormalizeHour(float time)
        {
            time %= 24f;
            if (time < 0f)
                time += 24f;

            return time;
        }
    }
}
