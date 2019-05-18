using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableCrateScript : MonoBehaviour
{
    public Vector3 startingPos;
    public Vector3 startingRot;

    private bool geyserBoolValue;

    public bool CrateInGeyser
    {
        get { return geyserBoolValue; }
        set
        {
            if (value == true)
            {
                geyserBoolValue = true;
                GetComponent<Rigidbody2D>().gravityScale = 0;
            }

            else if (value == false)
            {
                geyserBoolValue = false;
                GetComponent<Rigidbody2D>().gravityScale = 254.5f;
            }

        }
    }



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
