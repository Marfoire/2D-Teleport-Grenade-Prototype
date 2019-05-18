﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrenadeActionLeash : AbstractGrenadeAction
{
    //reference to the toss script of this grenade
    private GrenadeTossParabola tossScriptReference;

    //reference to the geyser prefab that is instantiated in grenade action
    public GameObject leashPrefab;

    //collision2d variable to hold collision data sent through during the on collision enter check
    Collision2D collidedSurface;

    public GameObject explosionPrefab;

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
        //TryCorrectGrenadePosition();

        //if the grenade collided with something that wasn't the stage
        if (collidedSurface.gameObject.tag == "Stage")
        {
            GameObject leash = Instantiate(leashPrefab, collidedSurface.gameObject.GetComponentInParent<Grid>().GetCellCenterWorld(collidedSurface.gameObject.GetComponentInParent<Grid>().WorldToCell(collidedSurface.GetContact(0).point + (collidedSurface.GetContact(0).separation * collidedSurface.GetContact(0).normal))), Quaternion.identity);       

            leash.GetComponent<ToolBehaviourLeash>().initiateCoroutine = true;
            leash.GetComponent<ToolBehaviourLeash>().playerReference = tossScriptReference.pScript;

            StartExplosion(GetComponentInChildren<SpriteRenderer>().color, 'L', explosionPrefab);
        }
        else
        {
            StartExplosion(GetComponentInChildren<SpriteRenderer>().color, 'S', explosionPrefab);
        }

        //set the player's bool restricting grenade throws to be false because the player should be able to throw grenades again
        tossScriptReference.pScript.preventGrenadeThrow = false;

        //destroy this grenade object
        Destroy(gameObject);
    }
}
