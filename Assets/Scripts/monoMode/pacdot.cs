using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class pacdot : NetworkBehaviour {
    public Text scoreUI;
    private string coName;
    private AudioSource source;

    void OnTriggerEnter2D(Collider2D co)
    {
        if(co.GetComponent<NetworkIdentity>())
        {
            this.coName = "multiPacman";
        } else {
            this.coName = "pacman";
        }
        if (co.name == this.coName)
        {
            source = co.GetComponent<AudioSource>();
            if (!source.isPlaying)
            {
                source.Play();
            } else
            {
                source.UnPause();
            }
            
            
            co.GetComponent<pacmanPlayer>().score += 10;
            Destroy(gameObject);
            
            scoreUI.text = "Score : " + co.GetComponent<pacmanPlayer>().score.ToString();
        }
    }
}
