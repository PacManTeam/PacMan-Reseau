using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class blinky_ia : NetworkBehaviour
{
    public Transform[] waypoints;
    int cur = 0;

    public float speed = 0.3f;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position != waypoints[cur].position)
        {
            Vector2 p = Vector2.MoveTowards(transform.position,
                                            waypoints[cur].position,
                                            speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }
        else
        {
            cur = (cur + 1) % waypoints.Length;
        }
        // Animation
        Vector2 dir = waypoints[cur].position - transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "pacman")
        {
            collision.GetComponent<pacmanPlayer>().healthPoint--;
            Instantiate<pacmanPlayer>(collision.GetComponent<pacmanPlayer>(), new Vector3(14,14,1), new Quaternion()).name = "pacman";
            Destroy(collision.gameObject);
        }
    }
}
