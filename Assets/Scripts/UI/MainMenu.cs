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
            SaveManager.LastSavedData.lastNight = 0;
            SaveManager.SaveGame();

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
