using System.Collections;
using System;
using System.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data;
using MySql.Data.MySqlClient;

public class authentification : MonoBehaviour {

    private MySqlConnection Connection;
    private MySqlDataAdapter MyAdapter;

    public void editFinish()
    {

        Debug.Log("Test");
    }

    public void tryToConnect()
    {
        Connection = new MySqlConnection("Database=BaseDeDonnees;Data Source=localhost;User Id=root;Password=password");
        Debug.Log("Connection");
        SceneManager.LoadScene("mainMenu");
    }
}
