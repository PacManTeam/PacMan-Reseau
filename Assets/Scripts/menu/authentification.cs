using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class authentification : MonoBehaviour {

    GameObject registerForm;
    GameObject signUpForm;

    public void Start()
    {
        this.registerForm = GameObject.Find("register");
        this.signUpForm = GameObject.Find("isSignUp");
        this.registerForm.SetActive(false);
    }

    public void editFinish()
    {

    }

    public void openRegisterForm()
    {
        if(this.signUpForm.activeInHierarchy)
        {
            GameObject.Find("signUp").GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Connexion";
            this.registerForm.SetActive(true);
            this.signUpForm.SetActive(false);
        } else
        {
            GameObject.Find("signUp").GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Inscription";
            this.registerForm.SetActive(false);
            this.signUpForm.SetActive(true);
        }

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
