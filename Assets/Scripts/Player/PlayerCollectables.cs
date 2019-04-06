using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectables : MonoBehaviour
{
    public int totalCollected;

    public void IncrementCollected() {
        //increment total by 1 nwhen collected
        totalCollected++;
    }
}
