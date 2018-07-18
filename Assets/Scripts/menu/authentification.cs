using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class authentification : MonoBehaviour {

    public void editFinish()
    {

    }

    public void tryToConnect()
    {
        Debug.Log("tryTo");
        this.testAuth();
        //StartCoroutine(this.testAuth());
    }

    private void testAuth()
    {
        Debug.Log("Authentification");
        SceneManager.LoadScene("mainMenu");
        /*WWWForm form = new WWWForm();
        form.AddField("login", "admin");
        form.AddField("password", "admin");

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/authentifier", form))
        {
            Debug.Log(www);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                SceneManager.LoadScene("mainMenu");
            }
        }*/
    }
}
