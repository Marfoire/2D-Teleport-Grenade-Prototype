using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour
{

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.GetComponentInChildren<SpriteRenderer>().color != transform.GetComponentInParent<SpriteRenderer>().color)
        {
            transform.GetComponentInChildren<SpriteRenderer>().color = transform.GetComponentInParent<SpriteRenderer>().color;
        }

        if (transform.GetComponentInParent<LineRenderer>().enabled)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward,transform.GetComponentInParent<LineRenderer>().GetPosition(1) - transform.position), 360);
        }
    }
}
