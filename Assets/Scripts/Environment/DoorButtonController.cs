using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonController : MonoBehaviour
{
    public bool DoorUseMotion;
    public bool DoorIsOn;
    public MotionDetection MD;
    public Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        DoorIsOn = false;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (DoorUseMotion == true)
        {
            anim.SetBool("DoorIsOn", MD.Open);
        }
    }

    
}
