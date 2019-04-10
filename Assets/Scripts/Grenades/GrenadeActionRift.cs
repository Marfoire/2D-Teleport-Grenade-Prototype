using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeActionRift : AbstractGrenadeAction
{
    //declare a variable to hold a reference to the toss script on this grenade
    private GrenadeTossParabola tossScriptReference;

    //reference to the rift prefab that is instantiated in grenade action
    public GameObject riftPrefab;
    public GameObject ExplsoinPrefab;

    //collision2d variable to hold collision data sent through during the on collision enter check
    Collision2D collidedSurface;

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
        //try to correct the position
        TryCorrectGrenadePosition();

        //if the grenade collided with something that wasn't the stage
        if (collidedSurface.gameObject.tag == "Stage")
        {
            //instantiate the rift tool with the respective prefab
            GameObject rift = Instantiate(riftPrefab, (Vector2)transform.position, Quaternion.identity);
            Instantiate(ExplsoinPrefab, this.transform.position, Quaternion.identity);
            //set the rift's initiate coroutine bool to true to start the call for it's behavioural couroutine, if it is directly called here, it will bug out when this is destroyed
            rift.GetComponent<ToolBehaviourRift>().initiateCoroutine = true;

            //get a player script reference
            rift.GetComponent<ToolBehaviourRift>().playerReference = tossScriptReference.pScript;

            //tell the player that there is now an active rift
            tossScriptReference.pScript.activeRift = true;
        }
        else
        {
            GameObject explosion = Instantiate(ExplsoinPrefab, this.transform.position, Quaternion.identity);
            explosion.transform.localScale = explosion.transform.localScale / 4;
        }

        //set the player's bool restricting grenade throws to be false because the player should be able to throw grenades again
        tossScriptReference.pScript.preventGrenadeThrow = false;

        //destroy this grenade object
        Destroy(gameObject);
    }
}
