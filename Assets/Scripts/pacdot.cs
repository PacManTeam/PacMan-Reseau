using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pacdot : MonoBehaviour {
    public Text scoreUI;

    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.name == "pacman")
        {
            co.GetComponent<scoreJoueur>().score++;
            Destroy(gameObject);
            scoreUI.text = "Score : " + co.GetComponent<scoreJoueur>().score.ToString();
        }
            
        Debug.Log(co);
    }
}
