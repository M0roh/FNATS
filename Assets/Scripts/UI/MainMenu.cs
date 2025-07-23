using UnityEngine;
using UnityEngine.SceneManagement;

namespace VoidspireStudio.FNATS.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void Continue() { }
        public void NewGame()
        {
            SceneManager.LoadScene(1);
        }
        public void Settings() { }
        public void Quit()
        {
            Application.Quit();
        }
    }
}
