using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Switch to Ingame Scene
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    // Quit
    public void Quit()
    {
        Application.Quit();
    }
        
    // Switch to Guide Scene
    public void Guide()
    {
        SceneManager.LoadScene(2);
    }

    // Switch to Author Scene
    public void Info()
    {
        SceneManager.LoadScene(3);
    }

    // Back to Menu Scene
    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}
