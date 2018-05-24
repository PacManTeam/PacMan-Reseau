using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour {

    public void playSolo()
    {
        SceneManager.LoadScene("soloMode");
    }

    public void playMulti()
    {
        SceneManager.LoadScene("multiMode");
    }

    public void quit()
    {
        Application.Quit();
    }
}
