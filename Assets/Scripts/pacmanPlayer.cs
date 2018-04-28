using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacmanPlayer : MonoBehaviour {

    public int score = 0;
    public int healthPoint = 3;
    public Vector2 spawnPosition = Vector2.zero;

	// Update is called once per frame
	void FixedUpdate () {
		if(healthPoint <= 0)
        {
            Destroy(gameObject);
        }
	}
}
