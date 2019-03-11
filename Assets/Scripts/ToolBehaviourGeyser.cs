using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBehaviourGeyser : MonoBehaviour
{
    //bool that establishes if the rise and fall coroutine should be run (this should only be true once)
    public bool initiateCoroutine;

    void Awake()
    {
        //if the geyser is not facing upwards
        if (transform.up != Vector3.up)
        {
            //the platform effector and the collider of the child should not be active
            transform.GetChild(0).GetComponent<PlatformEffector2D>().enabled = false;
            transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        }   
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //if the object is tagged as the player or a moveable object
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "MoveableObject")
        {
            //and if this geyser is not facing upwards
            if (transform.up != Vector3.up)
            {               
                //push the player with a weaker value
                other.gameObject.GetComponent<Rigidbody2D>().velocity = other.gameObject.GetComponent<Rigidbody2D>().velocity + ((Vector2)transform.up * 20);
            }
            else //if the geyser is facing upwards though
            {
                //push them with a stronger value
                other.gameObject.GetComponent<Rigidbody2D>().velocity = other.gameObject.GetComponent<Rigidbody2D>().velocity + ((Vector2)transform.up * 40);
            }
            
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if the player enters the geyser
        if (other.gameObject.tag == "Player")
        {
            //and if the geyser is facing upwards
            if (transform.up == Vector3.up)
            {
                //the player is in an upwards geyser so set their bool to true
                other.gameObject.GetComponent<PlayerController>().upwardsGeyser = true;
                //turn the player gravity off so they get shot up faster
                other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                //halve the player velocity to make it feel like they entered a stream of pressurized water
                other.gameObject.GetComponent<Rigidbody2D>().velocity = other.gameObject.GetComponent<Rigidbody2D>().velocity * 0.5f;
            }
            else//if the geyser is not facing upwards 
            {
                //apply a strong initial force on them
                other.gameObject.GetComponent<Rigidbody2D>().velocity = other.gameObject.GetComponent<Rigidbody2D>().velocity + ((Vector2)transform.up * 400);
            }

            //the player is now in the geyser so set the player's bool identifying that to true
            other.gameObject.GetComponent<PlayerController>().inGeyser = true;                     
        }

        //if a moveable object enters the geyser
        if(other.gameObject.tag == "MoveableObject")
        {
            //lower its gravity to the geyser pushes it harder
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if a player exits the geyser
        if (other.gameObject.tag == "Player")
        {
            //the geyser bools for the player should now be false
            other.gameObject.GetComponent<PlayerController>().upwardsGeyser = false;
            other.gameObject.GetComponent<PlayerController>().inGeyser = false;
            //reset the player's gravity
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 254.5f;
            
        }

        //if a moveable object exits the geyser
        if(other.gameObject.tag == "MoveableObject")
        {
            //reset that object's gravity
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 254.5f;
        }

    }

    private void Update()
    {
        //if the coroutine start bool is set to true by the grenade
        if (initiateCoroutine == true) {
            //start the geyser's coroutine to make it rise and fall
            StartCoroutine(RiseAndFall());
            //the coroutine should not be called a second time so set the bool to false
            initiateCoroutine = false;
        }
    }


    public IEnumerator RiseAndFall()
    {
        //while the geyser is not at full y size
        while (transform.localScale.y < 160)
        {
            //increase the geyser's y scale by 10
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + 10, transform.localScale.z);
            //add to the position of the geyser to keep it in place while it scales (this helps make it look like the scaling is only scaling one side of the geyser)
            transform.localPosition = transform.localPosition + (5 * transform.up);
            //wait very briefly before running again
            yield return new WaitForSecondsRealtime(0.008f);
        }

        //wait 10 seconds before moving on to the falling of the geyser
        yield return new WaitForSeconds(10);

        //while the geyser is still visible
        while (transform.localScale.y > 0)
        {
            //shrink the geyser by 10 on the y scale
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - 10, transform.localScale.z);
            //adjust the position so the geyser stays in the ground while it sinks
            transform.localPosition = transform.localPosition - (8 * transform.up);
            //wait very briefly before continuing
            yield return new WaitForSecondsRealtime(0.008f);
        }

        //destroy the geyser once it shrinks beyond visibility
        Destroy(gameObject);        
    }


}
