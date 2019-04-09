using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableCrateScript : MonoBehaviour
{
    public Vector3 startingPos;

    // Start is called before the first frame update
    void Awake()
    {
        startingPos = transform.position;
    }
}
