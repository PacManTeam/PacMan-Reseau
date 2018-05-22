using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class blinky_ia : NetworkBehaviour
{
    public Transform[] waypoints;
    int cur = 0;
    int backDirection;
    private Vector2 nextDist = Vector2.zero;
    int top = 0;
    int bottom = 1;
    int left = 2;
    int right = 3;

    public float speed = 5.9f;
    // Update is called once per frame
    void FixedUpdate()
    {

        System.Random aleatoire = new System.Random();
        int entierUnChiffre = aleatoire.Next(15);
        if (entierUnChiffre >= 4) entierUnChiffre = backDirection;

        while (!((entierUnChiffre == top && backDirection != bottom && valid(Vector2.up)) || (entierUnChiffre == bottom && backDirection != top && valid(Vector2.down)) || (entierUnChiffre == left && backDirection != right && valid(Vector2.left)) || (entierUnChiffre == right && backDirection != left && valid(Vector2.right))))
        {
            aleatoire = new System.Random();
            entierUnChiffre = aleatoire.Next(15);
            if (entierUnChiffre >= 4) entierUnChiffre = backDirection;
        }

        if (entierUnChiffre == top)
        {
            //Debug.Log("Je monte");
            //backDirection = new Vector2();
            //backDirection.x = 0;
            //backDirection.y = GetComponent<Rigidbody2D>().position.y + 1;
            Vector2 p = Vector2.MoveTowards(transform.position, (Vector2)transform.position + Vector2.up, speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }

        if (entierUnChiffre == bottom)
        {
            //Debug.Log("Je descends");
            //backDirection = new Vector2();
            //backDirection.x = 0;
            //backDirection.y = GetComponent<Rigidbody2D>().position.y - 1;
            Vector2 p = Vector2.MoveTowards(transform.position, (Vector2)transform.position + Vector2.down, speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }

        if (entierUnChiffre == left)
        {
            //Debug.Log("Je vais à gauche");
            //backDirection = new Vector2();
            //backDirection.x = GetComponent<Rigidbody2D>().position.x - 1;
            //backDirection.y = 0;
            Vector2 p = Vector2.MoveTowards(transform.position, (Vector2)transform.position + Vector2.left, speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }

        if (entierUnChiffre == right)
        {
            //Debug.Log("Je vais à droite");
            //backDirection = new Vector2();
            //backDirection.x = GetComponent<Rigidbody2D>().position.x + 1;
            //backDirection.y = 0;
            Vector2 p = Vector2.MoveTowards(transform.position, (Vector2)transform.position + Vector2.right, speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }



        backDirection = entierUnChiffre;

        //if (transform.position != waypoints[cur].position)
        //{
        //    Vector2 p = Vector2.MoveTowards(transform.position, waypoints[cur].position, speed);
        //    GetComponent<Rigidbody2D>().MovePosition(p);
        //}
        //else
        //{
        //    cur = (cur + 1) % waypoints.Length;
        //}
        // Animation
        //Vector2 dir = waypoints[cur].position - transform.position;
        //backDirection = dir;
        //GetComponent<Animator>().SetFloat("DirX", dir.x);
        //GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    public bool valid(Vector2 dir)
    {
        // Cast Line from 'next to Pac-Man' to 'Pac-Man'
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
        if (!(hit.collider == GetComponent<Collider2D>()))
        {
            nextDist = dir;
        }
        else
        {
            nextDist = Vector2.zero;
        }
        return (hit.collider == GetComponent<Collider2D>());
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
