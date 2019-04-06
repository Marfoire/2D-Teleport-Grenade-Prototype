using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the collision is with the player
        if (collision.gameObject.tag == "Player") {
            //increment the number collected
            collision.gameObject.GetComponent<PlayerCollectables>().IncrementCollected();
            //destroy the collectable
            Destroy(this.gameObject);
        }
    }

}
