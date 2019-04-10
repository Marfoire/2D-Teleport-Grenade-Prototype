using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IEnemy
{
    public float movementSpeed;
    public float timeAlive;
    private float timer;
    private float deathTimer;
    public float damage;
    public float attackSpeed;
    private GameObject target;
    
 
    public float enemyRadar;

    public bool enemyInGeyser;
    public bool upwardGeyserCheck;


    private Rigidbody2D rb;

    //declare the ColliderDistance2D check, this gives information on how to do hitbox correction
    public ColliderDistance2D stageCheck;


    //declare a contact filter
    ContactFilter2D filter;
    //declare a layer mask that uses the StageLayer
    LayerMask stageMask;

    RaycastHit2D groundedCheck;

    BoxCollider2D bc;

    public virtual void Awake()
    {
        // this timer is meant for the attack speed of the enemy. Again we don't need this at the moment
        timer += Time.deltaTime;
        // find object with the tag "player"
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        bc = GetComponent<BoxCollider2D>();


        //declare a contact filter
        filter = new ContactFilter2D();
        //declare a layer mask that uses the StageLayer
        stageMask = LayerMask.GetMask("StageLayer");

        //set the contact filter's layer mask to the stage mask and make sure it is using the mask
        filter.SetLayerMask(stageMask);
        filter.useLayerMask = true;
    }

    public virtual void FixedUpdate()
    {
            if (groundedCheck)
            {
                rb.gravityScale = 0;
            }
            else if (upwardGeyserCheck == false)
            {
                rb.gravityScale = 254.5f;
            }

        MoveEnemy();
        Die();

        // This is for attacking the player if we decide to add a health value to the player instead of instant death
        // keeping this just in case 
        //if (AttackRange())
       // {
            //AttackPlayer();
       // }

        
    }

    // if the distance between the player and the enemy is less than the enemy radar range, the enemy will follow the player.
    public virtual void MoveEnemy()
    {
        float distance = Vector2.Distance(target.transform.position, rb.position);

        if (distance <= enemyRadar)
        {
            // timer that counts up
            deathTimer += Time.deltaTime;

            RaycastHit2D leftWallCheck = Physics2D.Raycast(new Vector2((bc.bounds.center.x) - (bc.bounds.extents.x + 0.5f), bc.bounds.center.y - (bc.bounds.extents.y - 0.1f)), Vector2.up, (bc.bounds.extents.y * 2 - 0.8f), stageMask);
            RaycastHit2D rightWallCheck = Physics2D.Raycast(new Vector2((bc.bounds.center.x) + (bc.bounds.extents.x + 0.5f), bc.bounds.center.y - (bc.bounds.extents.y - 0.1f)), Vector2.up, (bc.bounds.extents.y * 2 - 0.8f), stageMask);

            if (target.transform.position.x - rb.position.x > 0 && rightWallCheck == false)
            {
                rb.velocity = new Vector2(100, rb.velocity.y);
            }
            else if (leftWallCheck == false)
            {
                rb.velocity = new Vector2(-100, rb.velocity.y);
            }

        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

    }


    // this is meant for the enemy attacking the player and cause damage to health -- if we stick with the one hit we probably won't need this.
    // just keeping this here in case 
    /*public virtual void OnTriggerEnter(Collider other)
    {
        if (player == null)
        {
            if (other.gameObject.GetComponent<Health>())
            {
                player = other.gameObject.GetComponent<Health>();
            }
        }
    }

    public virtual void AttackPlayer()
    {
        timer += Time.deltaTime;
        if (timer >= attackSpeed)
        {
            player.TakeDamage(damage);
            print("ATTACKING PLAYER");
            timer = 0;
        }

    }*/


    // timer for how long the enemy stays alive for. Can be taken off if we want the enemies to just stick around
    public virtual void Die()
    {
        
        /*if (deathTimer >= timeAlive)
        {
            Destroy(this.gameObject);
        }
        */
    }

    // this is the attack range for when the enemy can deal damage to the player. We do not need this at the moment
    // because of the enemy interface this has to stay in until it gets changed
    public bool AttackRange()
    {
        if (Vector2.Distance(target.transform.position, transform.position) <= 1.0f)
        {
            return true;
        }
        return false;


    }


    public void TryCorrectEnemyPosition()
    {
        

        //create an array that overlapping colliders can be sent to
        Collider2D[] stageGrenadeColliders = new Collider2D[10];

        //do an overlap collider check with the grenade's circle collider using the contact filter and outputting results to the previously declared array
        GetComponent<BoxCollider2D>().OverlapCollider(filter, stageGrenadeColliders);

        //for each collider that passes through the contact filter in the overlap collider check
        foreach (Collider2D incomingCollider in stageGrenadeColliders)
        {
            //if the collider is not null 
            if (incomingCollider != null && !incomingCollider.gameObject.GetComponentInParent<ToolBehaviourGeyser>())
            {
                //use the ColliderDistance2D to get information about the collider overlap, this compares the grenade circle collider to stage collider which comes in through incoming collider
                stageCheck = GetComponent<BoxCollider2D>().Distance(incomingCollider);

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

    void LateUpdate()
    {
        if (upwardGeyserCheck != true)
        {
            TryCorrectEnemyPosition();
        }       

        //Debug.DrawRay(new Vector2(0.1f + bc.bounds.center.x - (bc.bounds.extents.x), rb.position.y - (bc.bounds.extents.y + 0.1f)), Vector2.right * (bc.bounds.extents.x * 2 - 0.1f), Color.magenta);

        groundedCheck = Physics2D.Raycast(new Vector2(0.1f + bc.bounds.center.x - (bc.bounds.extents.x), rb.position.y - (bc.bounds.extents.y + 0.1f)), Vector2.right, bc.bounds.extents.x * 2 - 0.1f, stageMask);
    }


}
