using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class pacdot : NetworkBehaviour {
    public Text scoreUI;
    private string coName;

    void OnTriggerEnter2D(Collider2D co)
    {
        if(co.GetComponent<NetworkIdentity>())
        {
            this.coName = "pacman";
        } else {
            this.coName = "pacman";
        }
        if (co.name == this.coName)
        {
            co.GetComponent<pacmanPlayer>().score += 10;
            Destroy(gameObject);
            scoreUI.text = "Score : " + co.GetComponent<pacmanPlayer>().score.ToString();
        }
    }
}
