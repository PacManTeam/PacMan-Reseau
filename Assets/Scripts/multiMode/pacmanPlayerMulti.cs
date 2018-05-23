using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class pacmanPlayerMulti : NetworkBehaviour {

    [SyncVar]
    public int score;                           //Score du joueur
    [SyncVar]
    public int healthPoint;                     //Vie de Pacman
    private int id;
    
    // Use this for initialization
    void Start () {
        this.score = 0;
        this.healthPoint = 3;
        Debug.Log(this.GetComponent<NetworkIdentity>().netId);
	}

    // Update is called once per frame
    void Update () {
		
	}
}
