using UnityEngine;
using UnityEngine.SceneManagement;

namespace VoidspireStudio.FNATS.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void Continue()
        {

            LoadScreenScene.SceneToLoad = "GameScene";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }
        public void NewGame()
        {
            LoadScreenScene.SceneToLoad = "GameScene";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
        }
        public void Settings() { }
        public void Quit()
        {
            Application.Quit();
        }
    }
}
