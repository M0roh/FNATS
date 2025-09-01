using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.Interactables;
using VoidspireStudio.FNATS.PowerSystem;

namespace VoidspireStudio.FNATS.Cameras
{
    public class Monitor : MonoBehaviour, IInteractable, IElectricDevice
    {
        public static Monitor Instance { get; private set; }

        [Header("Загрузка")]
        [SerializeField] private Texture2D[] _loadAnimationFrames;
        [SerializeField, Min(1)] private int _loopCount = 5;
        

        public bool IsActive => throw new System.NotImplementedException();

        public float GetCurrentConsumption => throw new System.NotImplementedException();

        public Material _screenMaterial;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            MonitorReload();
        }

        public void MonitorReload()
        {
            if (PC.Instance.IsActive)
                _screenMaterial.mainTexture = SecurityCamerasManager.Instance.CurrentCamera.Texture;
            else
                _screenMaterial.mainTexture = null;
        }

        public void OnInteract()
        {
            if (PC.Instance.IsActive)
                SecurityCamerasManager.Instance.OpenCameras();
        }

        public void OnInteractEnd() { }

        public void TurnOn()
        {
            if (!PC.Instance.IsActive) return;

            _screenMaterial.color = new(255, 255, 255);

            StartCoroutine(PlayLoadScreen());
        }

        public void TurnOff()
        {
            if (PC.Instance.IsActive) return;

            _screenMaterial.color = new(0, 0, 0);

            MonitorReload();
        }

        public IEnumerator PlayLoadScreen()
        {
            if (_loadAnimationFrames != null)
            {
                for (int i = 0; i < _loopCount; i++)
                {
                    foreach (var frame in _loadAnimationFrames)
                    {
                        _screenMaterial.mainTexture = frame;
                        yield return null;
                    }
                }
            }

            MonitorReload();
        }
    }
}
