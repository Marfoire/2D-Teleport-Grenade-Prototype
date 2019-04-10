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
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector2)transform.position == endPosition && GetComponent<ParticleSystem>().particleCount == 0)
        {
            Destroy(gameObject);
        }
        if (speed != 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPosition, speed);
        }
    }
}
