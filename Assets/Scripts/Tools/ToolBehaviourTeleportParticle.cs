using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBehaviourTeleportParticle : MonoBehaviour
{
    public Vector2 endPosition;
    public Vector2 startPosition;
    float speed;

    private void Start()
    {
        speed = Vector2.Distance(endPosition,transform.position)/10;
        print("boat");
        //transform.position = /*(Vector2)transform.position +*/ Vector2.LerpUnclamped(transform.position, endPosition - (Vector2)transform.position, -1);

        Debug.DrawLine(Vector2.LerpUnclamped(transform.position, endPosition - (Vector2)transform.position, -0.2f), endPosition, Color.yellow, 10000000);
        Debug.DrawLine(Vector2.LerpUnclamped(transform.position, endPosition - (Vector2)transform.position, 0), endPosition, Color.green, 1000000);
        Debug.DrawLine(Vector2.LerpUnclamped(transform.position, endPosition - (Vector2)transform.position, 0.2f), endPosition, Color.blue, 1000000);
    }

    // Update is called once per frame
    void Update()
    {
        


        if ((Vector2)transform.position == endPosition)
        {
            //Destroy(gameObject);
        }
        if (speed != 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPosition, speed);
        }
    }
}
