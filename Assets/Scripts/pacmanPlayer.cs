using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacmanPlayer : MonoBehaviour {

    public int score = 0;
    public int healthPoint = 3;
    public Vector2 spawnPosition = Vector2.zero;

    // Update is called once per frame
    void FixedUpdate () {

        isAlive();
    }

    private void isAlive()
    {
        GameObject life1 = GameObject.Find("life1");
        GameObject life2 = GameObject.Find("life2");
        GameObject life3 = GameObject.Find("life3");

        if (life1 && healthPoint < 3)
        {
            Destroy(life1);
        }
        if (life2 && healthPoint < 2)
        {
            Destroy(life2);
        }
        if (healthPoint <= 0)
        {
            Destroy(gameObject);
            Destroy(life3);
        }
    }
}
