using System;
using UnityEngine;

namespace VoidspireStudio.FNATS.Nights
{
    public static class NightTime
    {
        public static float CurrentTime { get; private set; } = 23f;
        public static float TimeScale { get; private set; } = 1f / 60f;

        private static int _previousHalfStep = -1;

        public static event Action<float> OnHalfHourChanged;

        public static void Tick(float tickTime = 1f / 3600f)
        {
            CurrentTime += tickTime * TimeScale;

            if (CurrentTime >= 24f)
                CurrentTime -= 24f;

            int currentHalfStep = Mathf.FloorToInt(CurrentTime * 2f);

            if (currentHalfStep != _previousHalfStep)
            {
                _previousHalfStep = currentHalfStep;
                OnHalfHourChanged?.Invoke(CurrentTime);
            }
        }

        public static string GetFormattedTime()
        {
            int hour = Mathf.FloorToInt(CurrentTime) % 24;
            int minute = Mathf.FloorToInt((CurrentTime - hour) * 60f);
            return $"{hour:00}:{minute:00}";
        }

        public static void Reset(float startHour = 23f, float secondsPerGameHour = 60f)
        {
            CurrentTime = startHour;
            TimeScale = 1f / secondsPerGameHour;
            _previousHalfStep = -1;
        }
    }

}
