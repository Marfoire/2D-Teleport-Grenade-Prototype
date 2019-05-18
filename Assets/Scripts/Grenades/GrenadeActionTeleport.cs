using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeActionTeleport : AbstractGrenadeAction
{
    //declare a variable to hold a reference to the toss script on this grenade
    private GrenadeTossParabola tossScriptReference;

    //collision2d variable to hold collision data sent through during the on collision enter check
    private Collision2D collidedSurface;

    public GameObject particlePrefab;

    public GameObject explosionPrefab;

    private void Awake()
    {
        //get a reference to the toss script for this grenade
        tossScriptReference = GetComponent<GrenadeTossParabola>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //store the collision in the collided surface variable
        collidedSurface = collision;

        //call the grenade's action when it collides with something
        GrenadeAction();
    }


    public override void GrenadeAction()
    {
        //try to correct the grenade position before performing the grenade action
        TryCorrectGrenadePosition();
        
        //if the grenade collided with something that wasn't the stage
        if (collidedSurface.gameObject.tag == "Stage")
        {
            //transform.position = (Vector2)transform.position - Vector2.Lerp(transform.position, endPosition - (Vector2)transform.position, 0.2f);
            GameObject particles = Instantiate(particlePrefab, tossScriptReference.pScript.rb.position, Quaternion.identity);
            

            particles.GetComponent<ToolBehaviourTeleportParticle>().endPosition = GetComponent<Rigidbody2D>().position;
            particles.GetComponent<ToolBehaviourTeleportParticle>().startPosition = tossScriptReference.pScript.rb.position;

            //set the position of the player to the position of the grenade
            tossScriptReference.pScript.rb.position = GetComponent<Rigidbody2D>().position;

            StartExplosion(GetComponentInChildren<SpriteRenderer>().color, 'M', explosionPrefab);

            //update the player's transform as well for accuracy
            tossScriptReference.pScript.transform.position = tossScriptReference.pScript.rb.position;

            //set the player's correct position bool to true to check if they need to be ejected from a wall or floor
            tossScriptReference.pScript.correctMyPosition = true;

            //disable the player's double jump because that's cheating
            tossScriptReference.pScript.doubleJumpUsed = true;
        }
        else
        {
            StartExplosion(GetComponentInChildren<SpriteRenderer>().color, 'S', explosionPrefab);
        }

        //turn the prevent grenade throw bool off so the player can throw grenades again
        tossScriptReference.pScript.preventGrenadeThrow = false;

        //destroy this grenade game object
        Destroy(gameObject);
    }
}
