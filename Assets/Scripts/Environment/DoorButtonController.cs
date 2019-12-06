using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonController : MonoBehaviour
{
    public bool DoorUseMotion;
    public bool DoorIsOn;
    public MotionDetection MD;
    public Animator anim;
    public GameObject DoorCol;
    
    // Start is called before the first frame update
    void Start()
    {
        DoorIsOn = false;
        DoorCol.SetActive(true);
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        
        if (DoorUseMotion == true)
        {
            if (MD.Open == true)
            {
                anim.SetBool("DoorIsOn", MD.Open);
                DoorIsOn = true;
            }
            if (MD.Open == false)
            {
                anim.SetBool("DoorIsOn", MD.Open);
                DoorIsOn = false;
            }

        }

        if (DoorIsOn == false)
        {
            DoorCol.SetActive(true);
        }
        if (DoorIsOn == true)
        {
            DoorCol.SetActive(false);
        }
    }

    
}
