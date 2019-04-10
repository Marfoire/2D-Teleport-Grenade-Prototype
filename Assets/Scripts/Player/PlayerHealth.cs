using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public Transform checkpoint;

    public float deathDelayTimer = .1f; //how long it takes before the player is moved

    //CHANGE LATER
    private Color playerOgColour;

    public UnityEvent playerDied;

    private void Awake()
    {
        //set up the kill event
        playerDied = new UnityEvent();
    }


    public void PlayerDeath()
    {
        //CHANGE LATER
        //change the player's colour -> indicate hit?
        if(gameObject.GetComponent<SpriteRenderer>().color != Color.red)
        playerOgColour = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;

        //delay their reset
        StartCoroutine(DelayedMovePlayerBack(gameObject));   
    }

    private void MovePlayerToCheckpoint(GameObject player)
    {
        //move the player/rb to the last checkpoint
        player.transform.position = checkpoint.position;
        player.GetComponent<Rigidbody2D>().position = checkpoint.position;

        //CHANGE LATER
        //change player's colour back to green
        player.GetComponent<SpriteRenderer>().color = playerOgColour;
    }

    IEnumerator DelayedMovePlayerBack(GameObject player)
    {
        ///wait for the delay timer amount of time, then move the player back
        yield return new WaitForSeconds(deathDelayTimer);
        playerDied.Invoke();
        MovePlayerToCheckpoint(player);
        player.GetComponent<PlayerController>().preventGrenadeThrow = false;
        
    }

}
