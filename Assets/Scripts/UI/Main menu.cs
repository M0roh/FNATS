using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
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
