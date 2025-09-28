using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.Cameras {
    public class SecurityCamerasManager : MonoBehaviour
    {
        public static SecurityCamerasManager Instance { get; private set; }

        [SerializeField] private List<SecurityCamera> _securityCameras;
        [SerializeField] private RawImage _cameraView;
        [SerializeField] private TMP_Text _cameraName;

        private int _currentCameraIndex = 0;

        private Action<InputAction.CallbackContext>[] cameraDelegates;
        private InputAction[] _cameraInputs;

        public bool IsPlayerOnCameras { get; private set; }

        public SecurityCamera CurrentCamera => _securityCameras[_currentCameraIndex];

        public void Awake()
        {
            Instance = this;

            _cameraInputs = new InputAction[]
            {
                GameInput.Instance.InputActions.Camera.Camera1,
                GameInput.Instance.InputActions.Camera.Camera2,
                GameInput.Instance.InputActions.Camera.Camera3,
                GameInput.Instance.InputActions.Camera.Camera4,
                GameInput.Instance.InputActions.Camera.Camera5,
                GameInput.Instance.InputActions.Camera.Camera6,
                GameInput.Instance.InputActions.Camera.Camera7,
                GameInput.Instance.InputActions.Camera.Camera8,
                GameInput.Instance.InputActions.Camera.Camera9,
                GameInput.Instance.InputActions.Camera.Camera10
            };

            cameraDelegates = new Action<InputAction.CallbackContext>[_securityCameras.Count];

            for (int i = 0; i < _securityCameras.Count; i++)
            {
                int index = i;
                cameraDelegates[index] = (_) => ChangeCamera(index);
            }
        }

        public void OnEnable()
        {
            GameInput.Instance.InputActions.Camera.CloseCamera.performed += CloseCameras;

            GameInput.Instance.InputActions.Camera.CameraPrev.performed += PrevCamera;
            GameInput.Instance.InputActions.Camera.CameraNext.performed += NextCamera;

            for (int i = 0; i < _cameraInputs.Length; i++)
            {
                _cameraInputs[i].performed += cameraDelegates[i];
                _securityCameras[i].Deactivate();
            }
        }

        public void OnDisable()
        {
            GameInput.Instance.InputActions.Camera.CloseCamera.performed -= CloseCameras;

            GameInput.Instance.InputActions.Camera.CameraPrev.performed -= PrevCamera;
            GameInput.Instance.InputActions.Camera.CameraNext.performed -= NextCamera;

            for (int i = 0; i < _cameraInputs.Length; i++)
                _cameraInputs[i].performed -= cameraDelegates[i];
        }

        public void Update()
        {
            if (!IsPlayerOnCameras) return;

            _securityCameras[_currentCameraIndex].Look(GameInput.Instance.GetCameraLookVector());
        }

        public void OpenCameras()
        {
            IsPlayerOnCameras = true;
            _cameraView.gameObject.SetActive(true);
            GameInput.Instance.InputActions.Player.Disable();
            GameInput.Instance.InputActions.Camera.Enable();
            ReloadCamera();
        }

        public void CloseCameras(InputAction.CallbackContext _)
        {
            IsPlayerOnCameras = false;
            _cameraView.gameObject.SetActive(false);
            GameInput.Instance.InputActions.Player.Enable();
            GameInput.Instance.InputActions.Camera.Disable();
        }

        public void NextCamera(InputAction.CallbackContext _)
        {
            int nextIndex = _currentCameraIndex + 1;
            if (nextIndex >= _securityCameras.Count)
                nextIndex = 0;

            ChangeCamera(nextIndex);
        }

        public void PrevCamera(InputAction.CallbackContext _)
        {
            int prevIndex = _currentCameraIndex - 1;
            if (prevIndex < 0)
                prevIndex = _securityCameras.Count - 1;

            ChangeCamera(prevIndex);
        }

        public void ChangeCamera(int camIndex)
        {
            if (camIndex < 0 || camIndex >= _securityCameras.Count) return;

            CurrentCamera.Deactivate();
            _currentCameraIndex = camIndex;
            ReloadCamera();
        }

        public void ReloadCamera()
        {
            CurrentCamera.Activate();
            _cameraView.texture = CurrentCamera.Texture;
            _cameraName.text = CurrentCamera.Name;
            Monitor.Instance.MonitorReload();
        }
    } 
}