using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeActionDummy : AbstractGrenadeAction
{
    //declare a variable to hold a reference to the toss script on this grenade
    private GrenadeTossParabola tossScriptReference;

    //collision2d variable to hold collision data sent through during the on collision enter check
    private Collision2D collidedSurface;
    public GameObject ExplsoinPrefab;

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
        // just spawn the grenade failed prefab, this grenade isnt supposed to do anything
        GameObject explosion = Instantiate(ExplsoinPrefab, this.transform.position, Quaternion.identity);
        explosion.transform.localScale = explosion.transform.localScale / 4;

        //turn the prevent grenade throw bool off so the player can throw grenades again
        tossScriptReference.pScript.preventGrenadeThrow = false;

        //destroy this grenade game object
        Destroy(gameObject);
    }
}
