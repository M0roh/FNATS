using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Input;

namespace VoidspireStudio.FNATS.Cameras {
    public class SecurityCamerasManager : MonoBehaviour
    {
        public static SecurityCamerasManager Instance { get; private set; }

        [SerializeField] private List<SecurityCamera> _securityCameras;
        [SerializeField] private RawImage _cameraView;

        private int _currentCameraIndex = 0;

        private Action<InputAction.CallbackContext>[] cameraDelegates;

        public SecurityCamera CurrentCamera => _securityCameras[_currentCameraIndex];

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            cameraDelegates = new Action<InputAction.CallbackContext>[3];

            cameraDelegates[0] = (ctz) => ChangeCamera(0);
            cameraDelegates[1] = (ctz) => ChangeCamera(1);
        }

        public void OnEnable()
        {
            GameInput.Instance.InputActions.Camera.CloseCamera.performed += CloseCameras;

            GameInput.Instance.InputActions.Camera.CameraPrev.performed += NextCamera;
            GameInput.Instance.InputActions.Camera.CameraNext.performed += PrevCamera;

            GameInput.Instance.InputActions.Camera.Camera1.performed += cameraDelegates[0];
            GameInput.Instance.InputActions.Camera.Camera2.performed += cameraDelegates[1];
        }

        public void OnDisable()
        {
            GameInput.Instance.InputActions.Camera.CloseCamera.performed -= CloseCameras;

            GameInput.Instance.InputActions.Camera.CameraPrev.performed -= NextCamera;
            GameInput.Instance.InputActions.Camera.CameraNext.performed -= PrevCamera;

            GameInput.Instance.InputActions.Camera.Camera1.performed -= cameraDelegates[0];
            GameInput.Instance.InputActions.Camera.Camera2.performed -= cameraDelegates[1];
        }

        public void OpenCameras()
        {
            _cameraView.gameObject.SetActive(true);
            GameInput.Instance.InputActions.Player.Disable();
            GameInput.Instance.InputActions.Camera.Enable();
            ReloadCamera();
        }

        public void CloseCameras(InputAction.CallbackContext ctx)
        {
            _cameraView.gameObject.SetActive(false);
            GameInput.Instance.InputActions.Player.Enable();
            GameInput.Instance.InputActions.Camera.Disable();
        }

        public void NextCamera(InputAction.CallbackContext ctx)
        {
            _currentCameraIndex++;

            if (_currentCameraIndex >= _securityCameras.Count)
                _currentCameraIndex = 0;

            ReloadCamera();
        }

        public void PrevCamera(InputAction.CallbackContext ctx)
        {
            _currentCameraIndex--;

            if (_currentCameraIndex < 0)
                _currentCameraIndex = _securityCameras.Count - 1;

            ReloadCamera();
        }

        public void ChangeCamera(int camIndex)
        {
            if (camIndex < 0 || camIndex >= _securityCameras.Count) return;

            _currentCameraIndex = camIndex;
            ReloadCamera();
        }

        public void ReloadCamera()
        {
            _cameraView.texture = _securityCameras[_currentCameraIndex].Texture;
            Monitor.Instance.MonitorReload();
        }
    } 
}