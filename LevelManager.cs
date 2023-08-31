using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene("Canyon",LoadSceneMode.Single);
        //SceneManager.LoadScene(1);
    }    

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
        //SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        Scene currentScene=SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
