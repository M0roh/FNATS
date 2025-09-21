using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.UI.Menus.SettingsTabs
{
    public class GraphicsSettings : MonoBehaviour
    {
        [Header("Screen")]
        [SerializeField] private TMP_Dropdown _resolutionDropDown;
        [SerializeField] private Toggle _fullscreen;

        [Header("Brightness")]
        [SerializeField] private Slider _brightness;
        [SerializeField] private TMP_Text _brightnessText;
        
        [Header("FPS Cap")]
        [SerializeField] private Slider _fpsCap;
        [SerializeField] private TMP_Text _fpcCapText;
        [SerializeField] private LocalizedString _fpsNoLimitText;

        [Header("Other")]
        [SerializeField] private Toggle _motionBlur;
        [SerializeField] private Toggle _vSync;

        private readonly int[] _fpsCapOptions =  { 30, 45, 60, 120, 144, 160, 240, 300, -1 };

        private Exposure _exposure;

        private void Awake()
        {
            if (MainMenu.Instance.GlobalVolume.profile.TryGet<Exposure>(out var exp))
                _exposure = exp;
        }

        private void Start()
        {
            var graphicSettings = SaveManager.LastSavedData.graphics;

            _resolutionDropDown.ClearOptions();
            _resolutionDropDown.AddOptions(Screen.resolutions.Select((resolution) => $"{resolution.width}x{resolution.height} @ {resolution.refreshRateRatio}Hz").ToList());
            _resolutionDropDown.value = graphicSettings.resolutionIndex;

            _fullscreen.isOn = graphicSettings.isFullscreen;

            _brightness.value = graphicSettings.brightness;
            _brightnessText.text = $"{Mathf.RoundToInt(graphicSettings.brightness * 100)}%";

            _fpsCap.maxValue = _fpsCapOptions.Length - 1;
            _fpsCap.value = _fpsCapOptions.FirstOrDefault(fps => fps == graphicSettings.fpsCap);
            _fpcCapText.text = graphicSettings.fpsCap == -1 ? _fpsNoLimitText.GetLocalizedString() : graphicSettings.fpsCap.ToString();

            _motionBlur.isOn = graphicSettings.motionBlur;
            _vSync.isOn = graphicSettings.vSync;
        }

        private void OnEnable()
        {
            _resolutionDropDown.onValueChanged.AddListener(OnResolutionChange);
            _fullscreen.onValueChanged.AddListener(OnFullscreenChange);
            _brightness.onValueChanged.AddListener(OnBrightnessChange);
            _fpsCap.onValueChanged.AddListener(OnFPSCapChange);
            _motionBlur.onValueChanged.AddListener(OnMotionBlurToggle);
            _vSync.onValueChanged.AddListener(OnVSyncChange);
        }

        private void OnDisable()
        {
            _resolutionDropDown.onValueChanged.RemoveListener(OnResolutionChange);
            _fullscreen.onValueChanged.RemoveListener(OnFullscreenChange);
            _brightness.onValueChanged.RemoveListener(OnBrightnessChange);
            _fpsCap.onValueChanged.RemoveListener(OnFPSCapChange);
            _motionBlur.onValueChanged.RemoveListener(OnMotionBlurToggle);
            _vSync.onValueChanged.RemoveListener(OnVSyncChange);
        }

        public void OnResolutionChange(int resolutionIndex)
        {
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
            SaveManager.LastSavedData.graphics.resolutionIndex = resolutionIndex;
        }

        public void OnFullscreenChange(bool isFullscreen)
        {
            Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            SaveManager.LastSavedData.graphics.isFullscreen = isFullscreen;
        }

        public void OnBrightnessChange(float brightness)
        {
            _exposure.fixedExposure.value = (SaveManager.LastSavedData.graphics.brightness * -10f) + 5f;
            _brightnessText.text = $"{Mathf.RoundToInt(brightness * 100)}%";
            SaveManager.LastSavedData.graphics.brightness = brightness;
        }

        public void OnFPSCapChange(float fpsCapIndex)
        {
            int fpsIndex = Mathf.RoundToInt(fpsCapIndex);
            int fpsCap = _fpsCapOptions[fpsIndex];

            _fpcCapText.text = fpsCap == -1 ? _fpsNoLimitText.GetLocalizedString() : fpsCap.ToString();

            Application.targetFrameRate = SaveManager.LastSavedData.graphics.vSync ? -1 : fpsCap;
            SaveManager.LastSavedData.graphics.fpsCap = fpsCap;
        }

        public void OnMotionBlurToggle(bool motionBlur)
        {
            SaveManager.LastSavedData.graphics.motionBlur = motionBlur;
        }

        public void OnVSyncChange(bool vSync)
        {
            QualitySettings.vSyncCount = vSync ? 1 : 0;
            Application.targetFrameRate = vSync ? -1 : SaveManager.LastSavedData.graphics.fpsCap;
            SaveManager.LastSavedData.graphics.vSync = vSync;
        }
    }
}