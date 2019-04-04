using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationVeiwer : MonoBehaviour
{

    // to tell what direction the player is facing
    public bool FacingRight;
    public bool FacingLeft;
    // if the palyer is on ground
    public bool grounded;
    public bool Input;
    Animator animPAL;

    
    // Start is called before the first frame update
    void Start()
    {
        animPAL = gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        if(grounded == true)
        {
            animPAL.SetBool("grounded", true);
        }
        if (grounded == false)
        {
            animPAL.SetBool("grounded", false);
        }


        if (Input == true)
        {
            animPAL.SetBool("Input", true);
        }
        if (Input == false)
        {
            animPAL.SetBool("Input", false);
        }



        if (FacingRight == true)
        {
            animPAL.SetBool("IsFacingRight", true);
        }
        if (FacingRight == false)
        {
            animPAL.SetBool("IsFacingRight", false);
        }

        if (FacingLeft == true)
        {
            animPAL.SetBool("IsFacingLeft", true);
        }
        if (FacingLeft == false)
        {
            animPAL.SetBool("IsFacingLeft", false);
        }



    }

    public void checkPlayer()
    {

    }
}
