using System;
using System.Collections;
using UnityEngine;
using VoidspireStudio.FNATS.Cameras;

namespace VoidspireStudio.FNATS.Sounds.Game
{
    [AddComponentMenu("Scripts/Sounds/Game/SecurityCameraSounds")]
    public class SecurityCameraSounds : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip _cameraSwitchSound;
        [SerializeField] private AudioClip _cameraMovementSound;

        private bool _isNeedPlay = false;

        public void OnEnable()
        {
            SecurityCamerasManager.Instance.OnCameraChange += OnCameraChange; ;
            SecurityCamerasManager.Instance.OnCameraMovement += OnCameraMovement;
            StartCoroutine(PlayMoveSoundCooldown());
        }

        public void OnDisable()
        {
            SecurityCamerasManager.Instance.OnCameraChange -= OnCameraChange; ;
            SecurityCamerasManager.Instance.OnCameraMovement -= OnCameraMovement;
        }

        private void OnCameraChange()
        {
            AudioManager.Instance.PlaySound2D(_cameraSwitchSound, AudioManager.AudioType.SFX);
        }
        
        private void OnCameraMovement()
        {
            _isNeedPlay = true;
        }

        private IEnumerator PlayMoveSoundCooldown()
        {
            while (true)
            {
                if (_isNeedPlay)
                {
                    AudioManager.Instance.PlaySound2D(_cameraMovementSound, AudioManager.AudioType.SFX);
                    _isNeedPlay = false;
                    yield return new WaitForSeconds(_cameraMovementSound.length);
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}
