using UnityEngine;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class SecurityCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _maxVerticalAngle;
        [SerializeField] private float _maxHorizontalAngle;

        private Camera _camera;

        private float _verticalRotation;
        private float _horizontalRotation;

        public RenderTexture Texture => _camera.targetTexture;

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        }

        public void Look(Vector3 lookInput)
        {
            float mouseX = lookInput.x * GameSettings.MouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * GameSettings.MouseSensitivity * Time.deltaTime;

            _verticalRotation -= mouseY;
            _horizontalRotation -= mouseX;

            _verticalRotation = Mathf.Clamp(_verticalRotation, -_maxVerticalAngle, _maxVerticalAngle);
            _horizontalRotation = Mathf.Clamp(_horizontalRotation, -_maxHorizontalAngle, _maxHorizontalAngle);

            _camera.transform.localRotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0f);
        }
    }
}
