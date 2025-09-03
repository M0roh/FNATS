using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoidspireStudio.FNATS.Animatronics;

namespace VoidspireStudio.FNATS.Nights
{
    public class NightManager : SerializedMonoBehaviour
    {
        public static NightManager Instance { get; private set; }

        [OdinSerialize] private Dictionary<int, NightConfig> _nightConfigs = new();
        private NightConfig _currentConfig;

        private readonly Dictionary<AnimatronicAI, (int hour, int minute)> _animatronicActivate = new();
        private List<AnimatronicAI> _allAnimatronics = new();

        public int CurrentNight { get; set; } = 1;

        public NightConfig CurrentConfig => _currentConfig;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _allAnimatronics = FindObjectsByType<AnimatronicAI>(FindObjectsSortMode.None).ToList();
        }

        private void Start()
        {
            foreach (var animatronicTime in _currentConfig.AnimatronicActivity)
            {
                var animatronic = _allAnimatronics.FirstOrDefault(anim => anim.Id == animatronicTime.Key);

                if (animatronic == null)
                {
                    Debug.LogWarning($"Animatronic (ID = {animatronicTime.Key}) does not found.");
                    continue;
                }

                _animatronicActivate[animatronic] = animatronicTime.Value;
            }
        }

        private void OnEnable()
        {
            NightTime.OnTick += StartNight;
            NightTime.OnTick += NightComplete;
            NightTime.OnTick += AnimatronicsActivates;
        }

        private void AnimatronicsActivates(GameTime time)
        {
            foreach (var animatronicTime in _animatronicActivate)
            {
                if (time.IsTime(animatronicTime.Value.hour, animatronicTime.Value.minute))
                    animatronicTime.Key.Enable();
            }
        }

        private void OnDisable()
        {
            NightTime.OnTick -= StartNight;
            NightTime.OnTick -= NightComplete;
            NightTime.OnTick -= AnimatronicsActivates;
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
