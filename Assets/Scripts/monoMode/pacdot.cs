using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class pacdot : MonoBehaviour {
    public Text scoreUI;
    private string coName;
    private AudioSource source;

    void OnTriggerEnter2D(Collider2D co)
    {
        Debug.Log(co.name);
        if(co.name != "blinky" && co.name != "clyde" && co.name != "inky" && co.name != "pinky")
        {
            Destroy(this.gameObject);
            co.GetComponent<pacmanPlayer>().score += 10;
            //this.scoreUI.text = "Score :" + co.GetComponent<pacmanPlayer>().score.ToString();
            if (!co.GetComponent<AudioSource>().isPlaying)
            {
                co.GetComponent<AudioSource>().UnPause();
            }
            else
            {
                co.GetComponent<AudioSource>().Pause();
            }
        }
    }
}
