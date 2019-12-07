using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonController : MonoBehaviour
{
    public bool DoorUseMotion;
    public bool DoorIsOn;
    public MotionDetection MD;
    public Button BP;
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
        if(BP == null)
        {
            DoorUseMotion = true;
        }

        if (DoorUseMotion == false)
        {
            anim.SetBool("DoorIsOn", BP.ButtonPressed);
            if (BP.ButtonPressed == true)
            {
                DoorIsOn = true;
            }
            if (BP.ButtonPressed == false)
            {
                DoorIsOn = false;
            }
        }
        
        if (DoorUseMotion == true)
        {
            
            anim.SetBool("DoorIsOn", MD.Open);
            if (MD.Open == true)
            {
                DoorIsOn = true;
            }
            if (MD.Open == false)
            {   
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
