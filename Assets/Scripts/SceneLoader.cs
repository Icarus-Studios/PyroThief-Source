using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    
    public void loadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void loadUITest()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("UI_Playground");
    }

    public void loadBasicMovement()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BasicMovementAndAnim");
        
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
