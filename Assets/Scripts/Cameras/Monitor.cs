using UnityEngine;
using VoidspireStudio.FNATS.Input;
using VoidspireStudio.FNATS.Interactables;

namespace VoidspireStudio.FNATS.Cameras
{
    public class Monitor : MonoBehaviour, IInteractable
    {
        public static Monitor Instance { get; private set; }

        public Material _cameraPreview;

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

        public void MonitorReload() => _cameraPreview.mainTexture = SecurityCamerasManager.Instance.CurrentCamera.Texture;

        public void OnInteract() => SecurityCamerasManager.Instance.OpenCameras();

        public void OnInteractEnd() { }
    }
}
