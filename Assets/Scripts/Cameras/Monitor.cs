using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Interactables;
using VoidspireStudio.FNATS.Nights;
using VoidspireStudio.FNATS.PowerSystem;

namespace VoidspireStudio.FNATS.Cameras
{
    public class Monitor : MonoBehaviour, IInteractable, IElectricDevice
    {
        public static Monitor Instance { get; private set; }

        [SerializeField] private LocalizedString _interactTip;

        [Header("Загрузка")]
        [SerializeField] private Texture2D[] _loadAnimationFrames;
        [SerializeField, Min(1)] private int _loopCount = 3;
        [SerializeField, Min(0)] private float _delayBetweenFrames = 0.2f;

        private static Texture2D _blackTex;

        private bool _isLoaded = false;

        public bool IsActive => PC.Instance.IsActive;
        public bool CanInteract => IsActive;

        public LocalizedString InteractTip => _interactTip;

        public float GetCurrentConsumption => 0.01f * NightManager.Instance.CurrentNight;

        [SerializeField] private Material _screenMaterial;

        [Header("Индикация")]
        [SerializeField] private Material _indicatorMaterial;
        [SerializeField] private Light _lightIndicator;
        [SerializeField] private Color _onIndicator;
        [SerializeField] private Color _loadIndicator;
        [SerializeField] private Color _offIndicator;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (_blackTex == null)
            {
                _blackTex = new Texture2D(1, 1);
                _blackTex.SetPixel(0, 0, Color.black);
                _blackTex.Apply();
            }
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
                _screenMaterial.mainTexture = _blackTex;
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

            PlayLoadScreen().SuppressCancellationThrow().Forget();

            _lightIndicator.enabled = true;
            _indicatorMaterial.color = _loadIndicator;
            _lightIndicator.color = _loadIndicator;
        }

        public void TurnOff()
        {
            if (PC.Instance.IsActive) return;

            _isLoaded = false;

            if (PowerSystem.PowerSystem.Instance.IsStopped)
                _lightIndicator.enabled = false;
            _indicatorMaterial.color = _offIndicator;
            _lightIndicator.color = _offIndicator;

            MonitorReload();
        }

        public async UniTask PlayLoadScreen()
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
                        await UniTask.Delay(Mathf.RoundToInt(_delayBetweenFrames * 1000f), cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
                    }
                }
            }

            _screenMaterial.mainTextureOffset = new Vector2(0f, 0f);
            _screenMaterial.mainTextureScale = new Vector2(1f, 1f);

            _indicatorMaterial.color = _onIndicator;
            _lightIndicator.color = _onIndicator;

            MonitorReload();
            _isLoaded = true;
        }
    }
}
