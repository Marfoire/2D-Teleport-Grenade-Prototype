using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolBehaviourSingularity : MonoBehaviour
{

    public PlayerController playerReference;

    public bool initiateCoroutine;


    public void Start()
    {
        playerReference.overrideSingularity.AddListener(Dissipate);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().inSingularity = true;
            
        }
        else if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().enemyInSingularity = true;
        }

        collision.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().inSingularity = false;
            
        }
        else if(collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().enemyInSingularity = false;
        }

        collision.gameObject.GetComponent<Rigidbody2D>().gravityScale = 250;
    }

    private void Update()
    {
        if (initiateCoroutine)
        {
            StartCoroutine(ExpandAndDissipate());
            initiateCoroutine = false;
        }
    }

    public IEnumerator ExpandAndDissipate()
    {
        while (transform.localScale.x < 1)
        {
            transform.localScale = new Vector3(transform.localScale.x + 0.05f, transform.localScale.y + 0.05f, 1);
            yield return new WaitForSecondsRealtime(0.008f);
        }

        yield return new WaitUntil(() => transform.localScale.x >= 1);

        yield return new WaitForSecondsRealtime(15);

        while (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x - 0.05f, transform.localScale.y - 0.05f, 1);
            yield return new WaitForSecondsRealtime(0.008f);
        }

        yield return new WaitUntil(() => transform.localScale.x <= 0);

        Dissipate();

    }

    private void Dissipate()
    {
        Destroy(gameObject);
    }

}
