using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pacmanPlayer : MonoBehaviour {

    public int score = 0;                           //Score du joueur
    public int healthPoint = 3;                     //Vie de Pacman
    public Vector2 spawnPosition = Vector2.zero;    //Position de départ (ne sert à rien actuellement)

    // Méthode appeler toutes les frames affiché dans le jeu
    void FixedUpdate () {
        isAlive();
        GameObject maze = GameObject.Find("maze");
        if (GameObject.Find("maze").GetComponentInChildren<pacdot>() == null)
        {
            //SceneManager permet de gérer les scènes créer dans Unity
            SceneManager.LoadScene("soloMode");
            Debug.Log(this.score);
            GameObject.Find("pacman").GetComponentInChildren<pacmanPlayer>().score = this.score;
        }
    }

    /*
     * Mets à jour l'état de Pacman, ainsi que 
     * 
     */
    private void isAlive()
    {
        // On recherche les objects représentant les vies de pacman
        GameObject life1 = GameObject.Find("life1");
        GameObject life2 = GameObject.Find("life2");
        GameObject life3 = GameObject.Find("life3");

        // On agit en fonction du resultat de ces recherches et du nombre de points de vie du joueur
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
            SceneManager.LoadScene("soloMode");
        }
    }
}
