using UnityEngine;
using VoidspireStudio.FNATS.Nights;

namespace VoidspireStudio.FNATS.Core
{

    public class SunMoonController : MonoBehaviour
    {
        [Header("Объекты")]
        [SerializeField] private GameObject sun;
        [SerializeField] private GameObject moon;

        [Header("Настройки")]
        [SerializeField] private float transitionSmoothness = 2f;

        private Light sunLight;
        private Light moonLight;

        private void Awake()
        {
            sunLight = sun.GetComponent<Light>();
            moonLight = moon.GetComponent<Light>();
        }

        private void OnEnable()
        {
            NightTime.OnTick += UpdateSunMoon;
        }

        private void OnDisable()
        {
            NightTime.OnTick -= UpdateSunMoon;
        }

        private void UpdateSunMoon(GameTime time)
        {
            float currentHour = time.Hour + time.Minute / 60f;

            float sunAngle = currentHour / 24f * 360f - 90f;
            float moonAngle = sunAngle + 180f;

            sun.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);
            moon.transform.rotation = Quaternion.Euler(moonAngle, 0f, 0f);

            bool isDay = currentHour >= 6f && currentHour < 18f;

            sun.SetActive(isDay);
            moon.SetActive(!isDay);
        }

        private void Update()
        {
            if (sunLight != null)
                sunLight.intensity = Mathf.Lerp(sunLight.intensity, sun.activeSelf ? 1f : 0f, Time.deltaTime * transitionSmoothness);

            if (moonLight != null)
                moonLight.intensity = Mathf.Lerp(moonLight.intensity, moon.activeSelf ? 0.6f : 0f, Time.deltaTime * transitionSmoothness);
        }
    }
}