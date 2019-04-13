using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableCrateScript : MonoBehaviour
{
    public Vector3 startingPos;
    public Vector3 startingRot;

    // Start is called before the first frame update
    void Awake()
    {
        startingPos = transform.position;
        startingRot = transform.eulerAngles;
    }

    public void ResetPos()
    {
        transform.eulerAngles = startingRot;
        GetComponent<Rigidbody2D>().position = startingPos;
        transform.position = startingPos;
    }


}
