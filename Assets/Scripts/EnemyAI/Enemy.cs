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


    private Rigidbody2D rb;



   public virtual void Start()
    {
        // this timer is meant for the attack speed of the enemy. Again we don't need this at the moment
        timer += Time.deltaTime;
        // find object with the tag "player"
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void FixedUpdate()
    {
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
        float distance = Vector2.Distance(target.transform.position, transform.position);

        if (distance <= enemyRadar)
        {
            // timer that counts up
            deathTimer += Time.deltaTime;
            rb.transform.position = Vector2.MoveTowards(rb.transform.position, new Vector2(target.transform.position.x, rb.transform.position.y), movementSpeed * Time.deltaTime);
            
            

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
        
        if (deathTimer >= timeAlive)
        {
            Destroy(this.gameObject);
        }

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
}
