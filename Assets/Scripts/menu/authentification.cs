using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class authentification : MonoBehaviour {

    public void editFinish()
    {
        Debug.Log("Test");
    }

    public void tryToConnect()
    {
        Debug.Log("Connection");
        SceneManager.LoadScene("mainMenu");
    }
}
