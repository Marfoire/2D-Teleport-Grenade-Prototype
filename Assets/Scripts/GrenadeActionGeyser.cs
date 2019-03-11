using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeActionGeyser : AbstractGrenadeAction
{
    //reference to the toss script of this grenade
    private GrenadeTossParabola tossScriptReference;

    //reference to the geyser prefab that is instantiated in grenade action
    public GameObject geyserPrefab;

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

        //if the grenade collided with a moveable object, do not do the following
        if (collidedSurface.gameObject.tag != "MoveableObject")
        {
            //instantiate the geysey tool with the respective prefab
            GameObject geyser = Instantiate(geyserPrefab, (Vector2)transform.position - -stageCheck.normal, Quaternion.FromToRotation(transform.up, -stageCheck.normal));

            //set the geyser's initiate coroutine bool to true to start the call for it's behavioural couroutine, if it is directly called here, it will bug out when this is destroyed
            geyser.GetComponent<ToolBehaviourGeyser>().initiateCoroutine = true;
        }

        //set the player's bool restricting grenade throws to be false because the player should be able to throw grenades again
        tossScriptReference.pScript.preventGrenadeThrow = false;

        //destroy this grenade object
        Destroy(gameObject);
    }

    


}
