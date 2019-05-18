using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolBehaviourLeash : MonoBehaviour
{

    public PlayerController playerReference;
    public bool initiateCoroutine;
    private GameObject leashObject;
    public GameObject pointToAttachTo;
    public float travelSpeed;
    
    void Start()
    {

        leashObject = transform.GetChild(0).gameObject;

        leashObject.GetComponent<LineRenderer>().SetPosition(0, (Vector2)transform.position);
        leashObject.GetComponent<LineRenderer>().SetPosition(1, (Vector2)transform.position);

        playerReference.overrideLeash.AddListener(DeleteLeash);
    }

    // Update is called once per frame
    void Update()
    {
        if (initiateCoroutine)
        {
            StartCoroutine(LeashAttachAndHold());
            initiateCoroutine = false;
        }

        UpdateLeash();
    }

    void UpdateLeash()
    {
        if (pointToAttachTo != null)
        {
            float step = travelSpeed * Time.deltaTime;
            leashObject.GetComponent<LineRenderer>().SetPosition(1, Vector3.MoveTowards(leashObject.GetComponent<LineRenderer>().GetPosition(1), pointToAttachTo.transform.position, step));

            if (leashObject.GetComponent<DistanceJoint2D>().connectedBody == null && leashObject.GetComponent<LineRenderer>().GetPosition(1) == pointToAttachTo.transform.position)
            {                 
                leashObject.GetComponent<DistanceJoint2D>().connectedBody = pointToAttachTo.GetComponentInParent<Rigidbody2D>();
                leashObject.GetComponent<DistanceJoint2D>().connectedAnchor = pointToAttachTo.transform.localPosition;
            }       
        }

        if(leashObject.GetComponent<DistanceJoint2D>().connectedBody != null)
        {
            leashObject.GetComponent<LineRenderer>().SetPosition(1, pointToAttachTo.transform.position + ((pointToAttachTo.transform.position - leashObject.GetComponent<LineRenderer>().GetPosition(0)).normalized * 15));
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "LeashPoint" && pointToAttachTo == null)
        {
            pointToAttachTo = collision.gameObject;           
        }
    }

    public IEnumerator LeashAttachAndHold()
    {

        while (transform.localScale.x < 4100)
        {
            transform.localScale = new Vector3(transform.localScale.x + 100, transform.localScale.y + 100, 1);
            yield return new WaitForSecondsRealtime(0.008f);
        }

        yield return new WaitUntil(() => transform.localScale.x >= 4100);

        yield return new WaitForSecondsRealtime(1.5f);

        if(pointToAttachTo == null)
        {
            DeleteLeash();
        }
        
    }

    void DeleteLeash()
    {
        Destroy(gameObject);
    }

}
