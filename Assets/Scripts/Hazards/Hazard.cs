using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
	public Transform checkpoint; //where the player is moved back to
	public float deathDelayTimer = .1f; //how long it takes before the player is moved
	
	//CHANGE LATER
	private Color playerOgColour;
	
	//when the player enters the hazard's trigger collider
	private void OnTriggerEnter2D(Collider2D col)
	{
		//if the colliding object is the player
		if (col.gameObject.tag == "Player"){
            col.GetComponent<PlayerHealth>().PlayerDeath();
		}	

        if(col.gameObject.tag == "Enemy")
        {
            Destroy(col.gameObject);
        }
	}
	
}
