using UnityEngine;

namespace VoidspireStudio.FNATS.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Map state")]
        public bool IsLightOn { get; set; } = true;
        public bool NeedFuse { get; set; } = false;
        public bool DoorState { get; set; } = true;

        public int CurrentNight { get; set; } = 1;

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
    }
}
