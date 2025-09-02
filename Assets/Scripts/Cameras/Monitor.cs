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
        [SerializeField, Min(1)] private int _loopCount = 3;
        [SerializeField, Min(0)] private float _delayBetweenFrames = 0.2f; 

        private bool _isLoaded = false;

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
            if (PC.Instance.IsActive && _isLoaded)
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

            _isLoaded = false;
            _screenMaterial.color = new(0, 0, 0);

            MonitorReload();
        }

        public IEnumerator PlayLoadScreen()
        {
            _screenMaterial.mainTextureOffset = new Vector2(-0.5f, -0.5f);
            _screenMaterial.mainTextureScale = new Vector2(2f, 2f);
            if (_loadAnimationFrames != null)
            {
                for (int i = 0; i < _loopCount; i++)
                {
                    foreach (var frame in _loadAnimationFrames)
                    {
                        _screenMaterial.mainTexture = frame;
                        yield return new WaitForSeconds(_delayBetweenFrames);
                    }
                }
            }

            _screenMaterial.mainTextureOffset = new Vector2(0f, 0f);
            _screenMaterial.mainTextureScale = new Vector2(1f, 1f);

            MonitorReload();
            _isLoaded = true;
        }
    }
}
