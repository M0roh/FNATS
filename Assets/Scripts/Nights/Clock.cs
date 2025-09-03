using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void OnEnable()
        {
            NightTime.OnTick += ClockUpdate;
        }

        private void OnDisable()
        {
            NightTime.OnTick += ClockUpdate;
        }

        private void Update()
        {
            if (_minuteLerpProgress < 1f)
            {
                _minuteLerpProgress += Time.deltaTime / 1f;
                if (_minuteLerpProgress > 1f) _minuteLerpProgress = 1f;

                float angle = Mathf.LerpAngle(_startMinuteAngle, _targetMinuteAngle, _minuteLerpProgress);
                _minuteHand.localRotation = Quaternion.Euler(0, 0, angle);
            }

            float hourAngle = Mathf.LerpAngle(_startHourAngle, _targetHourAngle, _minuteLerpProgress);
            _hourHand.localRotation = Quaternion.Euler(0, 0, hourAngle);
        }

        public void ClockUpdate(GameTime time)
        {
            _startMinuteAngle = _minuteHand.localEulerAngles.z;
            _targetMinuteAngle = time.Minute / 60f * 360f;
            _minuteLerpProgress = 0f;

            _startHourAngle = _hourHand.localEulerAngles.z;
            _targetHourAngle = (time.Hour % 12) / 12f * 360f + (time.Minute / 60f) * 30f;
        }
    }
}
