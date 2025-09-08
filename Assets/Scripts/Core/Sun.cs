using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.Nights;

namespace VoidspireStudio.FNATS.Core
{
    using UnityEngine;

    public class SunMoonController : MonoBehaviour
    {
        [Header("Объекты")]
        [SerializeField] private GameObject sun;
        [SerializeField] private GameObject moon;

        [Header("Настройки")]
        [SerializeField] private float fullDayDuration = 120f;
        [SerializeField] private float transitionSmoothness = 2f;

        private Light sunLight;
        private Light moonLight;
        private float time;

        private void Awake()
        {
            sunLight = sun.GetComponent<Light>();
            moonLight = moon.GetComponent<Light>();

            sun.SetActive(true);
            moon.SetActive(false);
        }

        private void Update()
        {
            time += Time.deltaTime / fullDayDuration;
            if (time >= 1f) time = 0f;

            float sunAngle = time * 360f;
            float moonAngle = (time * 360f) + 180f;

            sun.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);
            moon.transform.rotation = Quaternion.Euler(moonAngle, 0f, 0f);

            float sunDot = Vector3.Dot(sun.transform.forward, Vector3.down);

            if (sunDot > 0)
            {
                if (!sun.activeSelf)
                {
                    sun.SetActive(true);
                    moon.SetActive(false);
                }
            }
            else
            {
                if (!moon.activeSelf)
                {
                    moon.SetActive(true);
                    sun.SetActive(false);
                }
            }

            if (sunLight != null && moonLight != null)
            {
                sunLight.intensity = Mathf.Lerp(sunLight.intensity, sun.activeSelf ? 1f : 0f, Time.deltaTime * transitionSmoothness);
                moonLight.intensity = Mathf.Lerp(moonLight.intensity, moon.activeSelf ? 0.6f : 0f, Time.deltaTime * transitionSmoothness);
            }
        }
    }
}