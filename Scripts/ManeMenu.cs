using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManeMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");

    }
    public void QuitGame()
    {
        Debug.Log("QuitGame");
        Application.Quit();

    }
}
