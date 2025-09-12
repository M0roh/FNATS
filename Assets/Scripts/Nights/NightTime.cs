using System;
using System.Collections;
using UnityEngine;

namespace VoidspireStudio.FNATS.Nights
{
    public class NightTime : MonoBehaviour
    {
        public static GameTime CurrentTime { get; private set; }
        public static float TimeScale { get; private set; } = 1f;

        public static event OnTickHandler OnTick;
        public delegate void OnTickHandler(GameTime time);

        private void OnEnable()
        {
            Reset();
            StartCoroutine(NightTimer());
        }

        public static void Reset(int startHour = 23)
        {
            CurrentTime = new(startHour);
            TimeScale = 1f;
        }

        public IEnumerator NightTimer()
        {
            do
            {
                CurrentTime.AddMinute();

                OnTick?.Invoke(CurrentTime);

                yield return new WaitForSeconds(TimeScale);
            } while (true);
        }
    }


    public class GameTime
    {
        private int _totalMinutes;

        public int Hour => _totalMinutes / 60;
        public int Minute => _totalMinutes % 60;

        public int TotalMinutes => _totalMinutes;

        public GameTime(int hour = 0, int minute = 0)
        {
            _totalMinutes = Normalize(hour * 60 + minute);
        }

        public void AddMinute()
        {
            _totalMinutes = Normalize(_totalMinutes + 1);
        }

        private static int Normalize(int minutes)
        {
            minutes %= 24 * 60;
            if (minutes < 0) minutes += 24 * 60;
            return minutes;
        }

        public string GetFormattedTime()
        {
            return $"{Hour:00}:{Minute:00}";
        }

        public bool IsTime(int hour, int minute)
        {
            return Hour == hour && Minute == minute;
        }
    }
}
