using UnityEngine;

namespace VoidspireStudio.FNATS.Nights
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private Transform _hourHand;
        [SerializeField] private Transform _minuteHand;

        private float _minuteLerpProgress = 1f;
        private float _startMinuteAngle;
        private float _targetMinuteAngle;

        private float _startHourAngle;
        private float _targetHourAngle;

        private const float LerpDuration = 1f;

        private void OnEnable()
        {
            NightTime.OnTick += ClockUpdate;
        }

        private void OnDisable()
        {
            NightTime.OnTick -= ClockUpdate;
        }

        private static float ToLogicalAngle(float appliedYRotation)
        {
            return Mathf.Repeat(-appliedYRotation, 360f);
        }

        private void Update()
        {
            if (_minuteLerpProgress < 1f)
            {
                _minuteLerpProgress += Time.deltaTime / LerpDuration;
                _minuteLerpProgress = Mathf.Min(_minuteLerpProgress, 1f);

                float minuteAngle = Mathf.LerpAngle(_startMinuteAngle, _targetMinuteAngle, _minuteLerpProgress);
                _minuteHand.localRotation = Quaternion.Euler(0f, -minuteAngle, 0f);

                float hourAngle = Mathf.LerpAngle(_startHourAngle, _targetHourAngle, _minuteLerpProgress);
                _hourHand.localRotation = Quaternion.Euler(0f, -hourAngle, 0f);
            }
        }

        public void ClockUpdate(GameTime time)
        {
            _targetMinuteAngle = (time.Minute / 60f) * 360f;
            _targetHourAngle = ((time.Hour % 12 + time.Minute / 60f) / 12f) * 360f;

            _startMinuteAngle = ToLogicalAngle(_minuteHand.localEulerAngles.y);
            _startHourAngle = ToLogicalAngle(_hourHand.localEulerAngles.y);

            _minuteLerpProgress = 0f;
        }
    }

}
