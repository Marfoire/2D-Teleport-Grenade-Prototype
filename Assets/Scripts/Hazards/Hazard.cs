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
			//MovePlayerToCheckpoint(col.gameObject);
			
			//delay their reset
			StartCoroutine(DelayedMovePlayerBack(col.gameObject));
			
			//CHANGE LATER
			//change the player's colour -> indicate hit?
			playerOgColour = col.gameObject.GetComponent<SpriteRenderer>().color;
			col.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
		}	
	}
	
	private void MovePlayerToCheckpoint(GameObject player){
		//move the player/rb to the last checkpoint
		player.transform.position = checkpoint.position;
		player.GetComponent<Rigidbody2D>().position = checkpoint.position;
		
		//CHANGE LATER
		//change player's colour back to green
		player.GetComponent<SpriteRenderer>().color = playerOgColour;
	}
	
	IEnumerator DelayedMovePlayerBack(GameObject player) {
		///wait for the delay timer amount of time, then move the player back
		yield return new WaitForSeconds(deathDelayTimer);
		MovePlayerToCheckpoint(player);
	}
	
}
