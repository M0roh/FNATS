using UnityEngine;
using UnityEngine.SceneManagement;

namespace VoidspireStudio.FNATS.UI {
    public class PauseMenu : MonoBehaviour
    {
        public void Continue()
        {
            Time.timeScale = 0f;
        }

        public void Settings()
        {

        }

        public void Credits()
        {

        }

        public void Quit()
        {
            LoadScreenScene.SceneToLoad = "MainMenu";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }
    }
}