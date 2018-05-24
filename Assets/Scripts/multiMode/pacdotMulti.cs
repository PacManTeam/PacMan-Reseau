using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacdotMulti : MonoBehaviour {
    private string coName;
    private AudioSource source;
    // Use this for initialization
    void Start () {
		
	}

    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.name != "blinky")
        {
            Destroy(this.gameObject);
        }
    }
}
