using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeActionLeash : AbstractGrenadeAction
{
    //reference to the toss script of this grenade
    private GrenadeTossParabola tossScriptReference;

    //reference to the geyser prefab that is instantiated in grenade action
    public GameObject leashPrefab;
    //public GameObject ExplsoinPrefab;

    //collision2d variable to hold collision data sent through during the on collision enter check
    Collision2D collidedSurface;

    private void Awake()
    {
        //set the toss script reference
        tossScriptReference = GetComponent<GrenadeTossParabola>();


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //store the collision in the collided surface variable
        collidedSurface = collision;

        //call the grenade action
        GrenadeAction();

    }


    public override void GrenadeAction()
    {
        //try to correct the position
        TryCorrectGrenadePosition();

        //if the grenade collided with something that wasn't the stage
        if (collidedSurface.gameObject.tag == "Stage")
        {

            GameObject leash = Instantiate(leashPrefab, (Vector2)transform.position, Quaternion.identity);
            //Instantiate(ExplsoinPrefab, transform.position, Quaternion.identity);

            //set the geyser's initiate coroutine bool to true to start the call for it's behavioural couroutine, if it is directly called here, it will bug out when this is destroyed
            leash.GetComponent<ToolBehaviourGeyser>().initiateCoroutine = true;
            leash.GetComponent<ToolBehaviourGeyser>().playerReference = tossScriptReference.pScript;
        }
        else
        {
            //GameObject explosion = Instantiate(ExplsoinPrefab, this.transform.position, Quaternion.identity);
            //explosion.transform.localScale = explosion.transform.localScale / 5.333f;
        }

        //set the player's bool restricting grenade throws to be false because the player should be able to throw grenades again
        tossScriptReference.pScript.preventGrenadeThrow = false;

        //destroy this grenade object
        Destroy(gameObject);
    }
}
