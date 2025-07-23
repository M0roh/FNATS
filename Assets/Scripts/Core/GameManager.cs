using UnityEngine;

namespace VoidspireStudio.FNATS.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Map state")]
        public bool IsLightOn { get; set; }
        public bool NeedFuse { get; set; }
        public bool DoorState { get; set; }

        public int CurrentNight { get; set; }

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
