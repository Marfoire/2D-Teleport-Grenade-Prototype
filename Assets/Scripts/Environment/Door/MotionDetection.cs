using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionDetection : MonoBehaviour
{

    public bool Open;
    
    
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Open = true;
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            Open = false;
        }
    }
}
