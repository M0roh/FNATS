using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoidspireStudio.FNATS.Animatronics;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.Nights
{
    public class NightManager : SerializedMonoBehaviour
    {
        public static NightManager Instance { get; private set; }

        [OdinSerialize] private Dictionary<int, NightConfig> _nightConfigs = new();
        private NightConfig _currentConfig;

        private readonly Dictionary<AnimatronicAI, ActivityTime> _animatronicActivate = new();
        private List<AnimatronicAI> _allAnimatronics = new();

        public int CurrentNight => SaveManager.LastSavedData.lastNight + 1;

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

            if (!_nightConfigs.TryGetValue(CurrentNight, out _currentConfig))
                Debug.LogError("Unknow night!");
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

                animatronic.Disable();
                _animatronicActivate[animatronic] = animatronicTime.Value;
            }
        }

        private void OnEnable()
        {
            NightTime.OnTick += StartNight;
            NightTime.OnTick += NightComplete;
            NightTime.OnTick += AnimatronicsActivates;
        }

        private void OnDisable()
        {
            NightTime.OnTick -= StartNight;
            NightTime.OnTick -= NightComplete;
            NightTime.OnTick -= AnimatronicsActivates;
        }

        private void AnimatronicsActivates(GameTime time)
        {
            foreach (var animatronicTime in _animatronicActivate)
            {
                if (time.IsTime(animatronicTime.Value.hour, animatronicTime.Value.minute))
                    animatronicTime.Key.Enable();
            }
        }

        public void StartNight(GameTime time)
        {
            if (!time.IsTime(0, 0)) return;

            PowerSystem.PowerSystem.Instance.StartConsumption();
        }

        public void NightComplete(GameTime time)
        {
            if (!time.IsTime(6, 0)) return;

            SaveManager.LastSavedData.lastNight++;
            SaveManager.SaveGame();
            Debug.Log("Ночь окончена!");
        }
    }
}
