using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class pacmanMoveMulti : NetworkBehaviour {

    [SyncVar]
    public float speed = 0.4f;
    [SyncVar]
    public Vector2 dest = Vector2.zero;
    [SyncVar]
    private Vector2 nextDist = Vector2.zero;

    Dictionary<string, bool> currentMove = new Dictionary<string, bool>();

    // Use this for initialization
    public void Start()
    {
        dest = transform.position;
        GetComponent<pacmanPlayer>().spawnPosition = dest;
        currentMove.Add("top", false);
        currentMove.Add("bottom", false);
        currentMove.Add("right", false);
        currentMove.Add("left", false);
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.UnloadSceneAsync("soloMode");
            SceneManager.LoadScene("mainMenu");
        }

        // Move closer to Destination
        Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        // Check for Input if not moving
        if (Input.GetKey(KeyCode.UpArrow) && valid(Vector2.up))
        {
            resetInput();
            currentMove["top"] = true;
        }
        if (Input.GetKey(KeyCode.RightArrow) && valid(Vector2.right))
        {
            resetInput();
            currentMove["right"] = true;
        }
        if (Input.GetKey(KeyCode.DownArrow) && valid(-Vector2.up))
        {
            resetInput();
            currentMove["bottom"] = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector2.right))
        {
            resetInput();
            currentMove["left"] = true;
        }

        if ((Vector2)transform.position == dest)
        {
            if (nextDist != Vector2.zero)
                dest = (valid(nextDist)) ? (Vector2)transform.position + nextDist : dest;
            if (currentMove["top"])
                dest = (valid(Vector2.up)) ? (Vector2)transform.position + Vector2.up : dest;
            if (currentMove["right"])
                dest = (valid(Vector2.right)) ? (Vector2)transform.position + Vector2.right : dest;
            if (currentMove["bottom"])
                dest = (valid(-Vector2.up)) ? (Vector2)transform.position - Vector2.up : dest;
            if (currentMove["left"])
                dest = (valid(-Vector2.right)) ? (Vector2)transform.position - Vector2.right : dest;
        }

        // Animation Parameters
        Vector2 dir = dest - (Vector2)transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
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

    public void resetInput()
    {
        currentMove["top"] = false;
        currentMove["bottom"] = false;
        currentMove["right"] = false;
        currentMove["left"] = false;
    }
}
