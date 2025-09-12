using UnityEngine;
using UnityEngine.Localization;
using VoidspireStudio.FNATS.Saves;

namespace VoidspireStudio.FNATS.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class SecurityCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LocalizedString _cameraName;
        [SerializeField] private float _maxVerticalAngle;
        [SerializeField] private float _maxHorizontalAngle;

        private Camera _camera;

        private float _verticalRotation = 0;
        private float _horizontalRotation = 0;
        private Quaternion _baseRotation;

        public RenderTexture Texture => _camera.targetTexture;
        public string Name => _cameraName.GetLocalizedString();

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            _baseRotation = _camera.transform.localRotation;
        }


        public void Look(Vector3 lookInput)
        {
            float mouseX = lookInput.x * SaveManager.LastSavedData.mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * SaveManager.LastSavedData.mouseSensitivity * Time.deltaTime;

            _verticalRotation -= mouseY;
            _horizontalRotation += mouseX;

            _verticalRotation = Mathf.Clamp(_verticalRotation, -_maxVerticalAngle, _maxVerticalAngle);
            _horizontalRotation = Mathf.Clamp(_horizontalRotation, -_maxHorizontalAngle, _maxHorizontalAngle);

            Quaternion verticalQuat = Quaternion.Euler(_verticalRotation, 0f, 0f);
            Quaternion horizontalQuat = Quaternion.Euler(0f, _horizontalRotation, 0f);

            _camera.transform.localRotation = _baseRotation * horizontalQuat * verticalQuat;
        }

        public void Activate() => _camera.enabled = true;
        public void Deactivate() => _camera.enabled = false;
    }
}
