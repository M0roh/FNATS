namespace VoidspireStudio.FNATS.Saves
{
    [System.Serializable]
    public class SaveData
    {
        public GraphicsSettings graphics = new();
        public AudioSettings audio = new();
        public GameplaySettings gameplay = new();
        public int lastNight = 0;
    }

    [System.Serializable]
    public class GraphicsSettings
    {
        public int resolutionIndex = -1;
        public bool isFullscreen = true;
        public float brightness = 0.5f;
        public int fpsCap = -1;
        public bool motionBlur = true;
        public bool vSync = true;
    }

    [System.Serializable]
    public class AudioSettings
    {
        public float volumeMusic = 0.5f;
        public float ambientVolume = 0.5f;
        public float volumeSFX = 0.5f;
    }

    [System.Serializable]
    public class GameplaySettings
    {
        public int languageIndex = 0;
        public float mouseSensitivity = 3f;
        public bool hints = true;
    }
}
