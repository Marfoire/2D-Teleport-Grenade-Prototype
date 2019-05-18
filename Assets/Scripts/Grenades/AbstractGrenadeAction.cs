using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGrenadeAction : MonoBehaviour
{
    //Grenade action method gets overriden with what grenades should do
    public abstract void GrenadeAction();

    //declare the ColliderDistance2D check, this gives information on how to do hitbox correction
    public ColliderDistance2D stageCheck;

    //try correct grenade position moves the grenade out of any colliders it overlaps with
    public void TryCorrectGrenadePosition()
    {
        //declare a contact filter
        ContactFilter2D filter = new ContactFilter2D();
        //declare a layer mask that uses the StageLayer
        LayerMask stageMask = LayerMask.GetMask("StageLayer");

        //set the contact filter's layer mask to the stage mask and make sure it is using the mask
        filter.SetLayerMask(stageMask);
        filter.useLayerMask = true;

        //create an array that overlapping colliders can be sent to
        Collider2D[] stageGrenadeColliders = new Collider2D[10];

        //do an overlap collider check with the grenade's circle collider using the contact filter and outputting results to the previously declared array
        GetComponent<CircleCollider2D>().OverlapCollider(filter, stageGrenadeColliders);

        //for each collider that passes through the contact filter in the overlap collider check
        foreach (Collider2D incomingCollider in stageGrenadeColliders)
        {
            //if the collider is not null 
            if (incomingCollider != null)
            {
                //use the ColliderDistance2D to get information about the collider overlap, this compares the grenade circle collider to stage collider which comes in through incoming collider
                stageCheck = GetComponent<CircleCollider2D>().Distance(incomingCollider);

                //if the check is definitely overlapping and it is valid
                if (stageCheck.isOverlapped && stageCheck.isValid)
                {
                    //Debug.DrawLine(stageCheck.pointB, stageCheck.pointA, Color.cyan);//draw a line to show the two points created THIS IS FOR DEBUGGING

                    //create the correction vector, this is done by taking the normal (direction) of the stage check and the distance of the stage check and multiplying them together
                    Vector2 correction = (stageCheck.normal * stageCheck.distance);
                    //adjust the grenade rigidbody to be properly corrected outside of the stage collider
                    GetComponent<Rigidbody2D>().position = GetComponent<Rigidbody2D>().position + correction;
                    //adjust the transform to the new rigidbody position for accuracy
                    transform.position = GetComponent<Rigidbody2D>().position;
                }
            }
        }
    }

    public void StartExplosion(Color c, char size, GameObject explosionObject)
    {

        if(size == 'L')
        {
            GameObject explosion = Instantiate(explosionObject, transform.position, Quaternion.identity);
            explosion.GetComponent<SpriteRenderer>().color = c;
        }

        else if (size == 'S')
        {
            GameObject explosion = Instantiate(explosionObject, transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector2(15,15);
            explosion.GetComponent<SpriteRenderer>().color = c;
        }

        else if (size == 'M')
        {
            GameObject explosion = Instantiate(explosionObject, transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector2(60, 60);
            explosion.GetComponent<SpriteRenderer>().color = c;
        }
    }



}
