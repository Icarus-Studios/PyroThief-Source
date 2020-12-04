using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    public void loadMainMenu()
    {
        Cursor.visible = true;
        Time.timeScale = 1f;
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

    public void loadCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void reloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
