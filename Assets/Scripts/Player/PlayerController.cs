using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{

    //horizontal movement variables
    public float storedLastHorizontalInput;//stores the last horizontal direction (axis raw) inputted by the player until deceleration is done
    public float MaxSpeed;//max horizontal speed of the player
    public float AccelerationTime;//time it takes to reach top speed
    public float DecelerationTime;//time it takes to decelerate to 0 speed
    public float totalVelocity;//total velocity after adding delta velocity to previous velocity
    public float deltaVelocity;//change in player velocity
    public float prevVelocity;//stored velocity of the last fixed update loop

    //jump variables (THESE VARIABLES WILL BE SUBJECT TO CHANGE IN THE NEAR FUTURE, A LOT OF THEM ARE NOT USED)
    public float gravityValue;//gravity value of the player
    public float jumpHeight;//jump height of the player
    public float jumpTime;//time it takes the player to reach their jump height
    public float jumpVelocity;//velocity applied to the player when they jump
    public float terminalVelocity;//max downwards velocity of the player
    public bool grounded;//grounded bool for the player

    //reference variables
    public Rigidbody2D rb;//rigidbody reference
    public BoxCollider2D bc;//boxcollider reference


    RaycastHit2D groundedCheck;//raycast top check if the player is grounded
    RaycastHit2D leftWallCheck;//raycast check to the left to see if the 
    RaycastHit2D rightWallCheck;//raycast check to the right to see if the 

    //bool to determine if collision correction should be attempted
    public bool correctMyPosition;

    //input bools to check if the player is inputting anything
    public bool jumpInputted;// if jump is pressed
    public bool leftInputted;// if left is pressed
    public bool rightInputted;// if right is pressed

    //booleans for if the player is jumping or if the player has used a double jump
    public bool isJumping;
    public bool doubleJumpUsed;

    //variable that stores the current grenade type and an array that holds all of the grenade types so they can be cycled through
    public GameObject currentGrenadePrefab;
    public GameObject[] grenadeTypePrefabs;

    //boolean that stops the grenade from 
    public bool preventGrenadeThrow;

    //boolean that is true when the player is inside a geyser
    public bool inGeyser;

    //boolean that is true when the player is inside a geyser that is pushing them upwards
    public bool upwardsGeyser;

    public bool inSingularity;

    //float that holds the throw height of the grenade parabola, defaults to 150
    public float throwHeight;

    //contact filter and layer mask for stage collision detection and correction purposes
    ContactFilter2D filter;
    LayerMask stageMask;

    //if the player is in the rift
    public bool inRift;

    //if there is an active rift in the scene
    public bool activeRift;

    

    //event to delete any active rifts when a new rift grenade is thrown
    public UnityEvent overrideRift;

    public UnityEvent overrideLeash;

    public UnityEvent overrideSingularity;

    // the animator
    Animator animPAL;
    // the shooting arm
    public GameObject rotatingArm;

    private bool faceValue;
    public bool LRF
    {
        get { return faceValue; }
        set
        {
            if (faceValue != value)
            {
                rotatingArm.transform.localPosition = new Vector3(-rotatingArm.transform.localPosition.x, rotatingArm.transform.localPosition.y, 0);
            }

            faceValue = value;
          
        }
    }



    void Awake()
    {
        //set up the rift event
        overrideRift = new UnityEvent();

        overrideLeash = new UnityEvent();

        overrideSingularity = new UnityEvent();

        //make sure the contact filter is a contact filter
        filter = new ContactFilter2D();

        //make sure the layer mask is a layer mask
        stageMask = LayerMask.GetMask("StageLayer");

        //set the contact filter up so it uses a layermask and so the layermask it uses is the stage mask
        filter.SetLayerMask(stageMask);
        filter.useLayerMask = true;

        //get references for the rigidbody and box collider
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        //hardcoded jump velocity because of the switch from in-script physics to physics engine (clean this up later)
        jumpVelocity = 900;

        //default throw height
        throwHeight = 150;

        // get refrence to the animator
        animPAL = gameObject.GetComponent<Animator>();
    }

    //method for jumping
    void JumpCall()
    {

        if (!doubleJumpUsed && !grounded && jumpInputted && inGeyser == false && inSingularity == false)
        {
            rb.velocity = Vector2.zero;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpVelocity);
            doubleJumpUsed = true;
        }

        //if the player is grounded and they press jump
        if ((jumpInputted == true && grounded == true))
        {

            if (jumpInputted == true && grounded == true)
            {
                isJumping = true;
            }

            //apply the initial jump velocity
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpVelocity);
        }
    }

    //handles horizontal movement, needs revamping
    public void HorizontalMovement()
    {
        // if there is no input then the input bool in the animator is false
        animPAL.SetBool("Input", false);
        //if the either of the input axis are receiving RAW input, start running some acceleration calculations (GetAxisRaw only returns 0,1,-1 based on what input is pressed)
        if (leftInputted == true || rightInputted == true)
        {
            // setting the input value in animator to true
            animPAL.SetBool("Input", true);
            //If the acceleration time is not 0
            if (AccelerationTime != 0)
            {
                deltaVelocity = MaxSpeed / AccelerationTime * Time.fixedDeltaTime;//find acceleration delta velocity with maxSpeed divided by acceleration time
                totalVelocity = prevVelocity + deltaVelocity;//add the delta velocity to the previous velocity to get the current total velocity                
            }
            else if (AccelerationTime == 0)  //if the acceleration time is 0, don't worry about doing acceleration calculations
            {
                totalVelocity = MaxSpeed;//set the velocity to the max speed
            }

        }

        //if there is no input being detected and the velocity isn't 0
        else if (leftInputted == false && rightInputted == false && totalVelocity > 0)
        {
            //if deceleration time is not 0
            if (DecelerationTime != 0)
            {
                deltaVelocity = MaxSpeed / DecelerationTime * Time.fixedDeltaTime;//find deceleration delta velocity with max speed divided by deceleration time 
            }
            else//if the deceleration is 0
            {
                deltaVelocity = prevVelocity;//don't worry about any deceleration calculations
            }
            totalVelocity = prevVelocity - deltaVelocity;//subtract the delta velocity from the previous velocity to get the current velocity value
        }

        //run a check to see if the velocity is greater than the speed cap
        if (totalVelocity > MaxSpeed)
        {
            totalVelocity = MaxSpeed;//correct the velocity back to the max speed
        }

        //if total velocity rolls over to be negative when decelerating
        if (totalVelocity < 0)
        {
            //storedLastHorizontalInput = 0;//reset the input to 0
            prevVelocity = (deltaVelocity + totalVelocity);
            totalVelocity = 0;//set the total velocity back to 0 to fix that
        }

        if (inGeyser == false && inSingularity == false && rb.velocity.x != 0)
        {
            if (rb.velocity.x > -15 && rb.velocity.x < 15)
                rb.velocity = new Vector2(0, rb.velocity.y);

            if (grounded == true)
            {
                rb.drag = 3;
            }
            if ((rb.velocity.x > 0 && storedLastHorizontalInput < 0) || (rb.velocity.x < 0 && storedLastHorizontalInput > 0))
            {
                rb.velocity = new Vector2(rb.velocity.x / 1.1f, rb.velocity.y);
            }
        }


        rb.position = new Vector2(rb.position.x + (totalVelocity * Time.fixedDeltaTime * storedLastHorizontalInput), rb.position.y);


        if (totalVelocity == 0)
        {
            storedLastHorizontalInput = 0;//reset the input to 0
        }


        //set the previous velocity to the current total velocity for the next loop around with this function
        prevVelocity = totalVelocity;
    }

    public void TryCorrectPosition(Collider2D[] incomingColliders)
    {
        foreach (Collider2D incomingCollider in incomingColliders)
        {
            if ((incomingCollider != null && incomingCollider.tag != "Geyser" && GetComponent<SpriteRenderer>().color != Color.red && incomingCollider.isTrigger == false) || (incomingCollider != null && incomingCollider.tag == "Geyser" && upwardsGeyser == false && GetComponent<SpriteRenderer>().color != Color.red && incomingCollider.isTrigger == false))
            {
                //print("epic victory royale");
                ColliderDistance2D stageCheck = bc.Distance(incomingCollider);
                if (stageCheck.isOverlapped && stageCheck.isValid)
                {
                    Debug.DrawLine(stageCheck.pointB, stageCheck.pointA, Color.cyan);
                    Vector2 correction = (stageCheck.normal * stageCheck.distance);
                    rb.position = rb.position + correction;
                    transform.position = rb.position;
                }
            }
        }
    }

    //checkGrounded does some raycasting and position correcting for the purpose of keeping the player grounded
    void CheckGrounded()
    {
        if (groundedCheck.collider != null && !groundedCheck.collider.isTrigger)
        {//if the ray collider with something

            if (upwardsGeyser == false)//if the player isn't being pushed up by a geyser
            {
                grounded = true;//set grounded to true
                // set the grounded bool in the animator to true
                animPAL.SetBool("grounded", true);
                doubleJumpUsed = false;
                isJumping = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.gravityScale = 0;

            }
        }
        else//if there are no collisions
        {
            grounded = false;//set grounded to false
            // set the grounded bool in the animator to false
            animPAL.SetBool("grounded", false);
            if (inGeyser == false && inSingularity == false)
            {
                rb.gravityScale = 254.5f;
            }

        }
    }

    public void DrawThrowableLine()
    {
        if (GetComponent<LineRenderer>().enabled)
        {
            float count = 20;
            GetComponent<LineRenderer>().positionCount = (int)count + 1;
            for (float i = 0; i < count + 1; i++)
            {
                Vector3 p = MapParabola(transform.GetChild(0).transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), throwHeight, i / count);
                GetComponent<LineRenderer>().SetPosition((int)i, new Vector3(p.x, p.y, -1));
            }
        }
    }

    Vector3 MapParabola(Vector3 startPoint, Vector3 endPoint, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(startPoint.y - endPoint.y) < 20)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = endPoint - startPoint;
            Vector3 result = startPoint + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = endPoint - startPoint;
            Vector3 levelDirection = endPoint - new Vector3(startPoint.x, endPoint.y, startPoint.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirection);
            Vector3 up = Vector3.Cross(right, levelDirection);
            if (endPoint.y > startPoint.y) up = -up;
            Vector3 result = startPoint + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }


    private void CheckInputs()
    {
        if (Input.GetMouseButtonDown(0) && preventGrenadeThrow == false)
        {
            preventGrenadeThrow = true;
            GameObject nadeObject = Instantiate(currentGrenadePrefab, transform.position, Quaternion.identity);
            nadeObject.GetComponent<GrenadeTossParabola>().GetInfo(this);
            GetComponent<LineRenderer>().enabled = false;

            if (activeRift == true && currentGrenadePrefab.name == "Grenade_Rift")
            {
                overrideRift.Invoke();
            }
            else if(currentGrenadePrefab.name == "Grenade_Leash")
            {
                overrideLeash.Invoke();
            }
            else if (currentGrenadePrefab.name == "Grenade_Singularity")
            {
                overrideSingularity.Invoke();
            }

        }

        if (Input.GetButtonDown("Jump") == true)
        {
            jumpInputted = true;
        }

        if (Input.GetKey("left") == true || Input.GetKey("a"))
        {
            if (!leftWallCheck || leftWallCheck.collider.isTrigger)
                leftInputted = true;

            // setting bools for the animator
            animPAL.SetBool("IsFacingRight", false);
            animPAL.SetBool("IsFacingLeft", true);
            LRF = true;

            storedLastHorizontalInput = Input.GetAxisRaw("Horizontal");//store the input values in a vector2 for deceleration
        }

        if (Input.GetKey("right") == true || Input.GetKey("d"))
        {
            if (!rightWallCheck || rightWallCheck.collider.isTrigger)
                rightInputted = true;

            // setting bools for the animator
            animPAL.SetBool("IsFacingRight", true);
            animPAL.SetBool("IsFacingLeft", false);
            LRF = false;

            storedLastHorizontalInput = Input.GetAxisRaw("Horizontal");//store the input values in a vector2 for deceleration
        }

        if (Input.GetMouseButtonDown(1))
        {
            SwitchToNextGrenade();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            throwHeight = Mathf.Clamp(throwHeight + (Input.mouseScrollDelta.y * 2), 150, 225);
        }

        if (preventGrenadeThrow == false)
        {
            GetComponent<LineRenderer>().enabled = true;
        }

    }

    private void SwitchToNextGrenade()
    {
        for (int i = 0; i < grenadeTypePrefabs.Length; i++)
        {
            if (grenadeTypePrefabs[i] == currentGrenadePrefab)
            {
                if (i + 1 < grenadeTypePrefabs.Length)
                {
                    currentGrenadePrefab = grenadeTypePrefabs[i + 1];
                }
                else
                {
                    currentGrenadePrefab = grenadeTypePrefabs[0];
                }
                Color currentGrenadeColour = currentGrenadePrefab.GetComponentInChildren<SpriteRenderer>().color;
                GetComponent<SpriteRenderer>().color = currentGrenadeColour;
                GetComponent<LineRenderer>().startColor = new Color(currentGrenadeColour.r, currentGrenadeColour.g, currentGrenadeColour.b, GetComponent<LineRenderer>().startColor.a);
                GetComponent<LineRenderer>().endColor = new Color(currentGrenadeColour.r, currentGrenadeColour.g, currentGrenadeColour.b, GetComponent<LineRenderer>().endColor.a);
                break;
            }
        }
    }


    private void CheckForLatePositionCorrection()
    {
        Collider2D[] stageColliders = new Collider2D[10];
        if (correctMyPosition == false)
        {
            if (bc.OverlapCollider(filter, stageColliders) > 0)
            {
                correctMyPosition = true;
            }
        }

        if (correctMyPosition == true)
        {
            bc.OverlapCollider(filter, stageColliders);
            TryCorrectPosition(stageColliders);
        }
        correctMyPosition = false;
    }

    private void CastWallAndGroundedChecks()
    {
        //these lines are for visualizing the rays that are casted below
        /*Debug.DrawRay(new Vector2(0.1f + bc.bounds.center.x - (bc.bounds.extents.x), rb.position.y - (bc.bounds.extents.y + 0.1f)), Vector2.right * (bc.bounds.extents.x * 2 - 0.1f), Color.white);
        Debug.DrawRay(new Vector2((rb.position.x) - (bc.bounds.extents.x + 0.5f), rb.position.y - (bc.bounds.extents.y - 0.1f)), Vector2.up * ((bc.bounds.extents.y * 2 - 0.8f)), Color.yellow);
        Debug.DrawRay(new Vector2((rb.position.x) + (bc.bounds.extents.x + 0.5f), rb.position.y - (bc.bounds.extents.y - 0.1f)), Vector2.up * ((bc.bounds.extents.y * 2 - 0.8f)), Color.yellow);
        */

        groundedCheck = Physics2D.Raycast(new Vector2(0.1f + bc.bounds.center.x - (bc.bounds.extents.x), rb.position.y - (bc.bounds.extents.y + 0.1f)), Vector2.right, bc.bounds.extents.x * 2 - 0.1f, stageMask);
        leftWallCheck = Physics2D.Raycast(new Vector2((bc.bounds.center.x) - (bc.bounds.extents.x + 0.5f), bc.bounds.center.y - (bc.bounds.extents.y - 0.1f)), Vector2.up, (bc.bounds.extents.y * 2 - 0.8f), stageMask);
        rightWallCheck = Physics2D.Raycast(new Vector2((bc.bounds.center.x) + (bc.bounds.extents.x + 0.5f), bc.bounds.center.y - (bc.bounds.extents.y - 0.1f)), Vector2.up, (bc.bounds.extents.y * 2 - 0.8f), stageMask);
    }



    //if the player collides with something try and correct their position
    private void OnCollisionEnter2D(Collision2D collision)
    {
        correctMyPosition = true;
    }

    //if the player is still colliding with something try and correct their position
    private void OnCollisionStay2D(Collision2D collision)
    {
        correctMyPosition = true;
    }

    void FixedUpdate()
    {
        //reset the player's drag at the start of fixed update, this is necessary because horizontal movement conditionally changes the drag of the player
        rb.drag = 1.4f;

        //check if the player is grounded
        CheckGrounded();

        

        //call horizontal movement to check and ensure the player is moving if they are trying to move horizontally
        HorizontalMovement();

        correctMyPosition = true;
        CheckForLatePositionCorrection();

        //run jump call to make sure the player jumps if they input it
        JumpCall();

        //turn all the inputted bools off in case the player is done inputting a specific action
        leftInputted = false;
        rightInputted = false;
        jumpInputted = false;
    }


    private void LateUpdate()
    {

        //run once last check to see if position correction should be done
        CheckForLatePositionCorrection();

        //case the wall and grounded raycast checks because they should be in line with the position of the player after all the other operations
        CastWallAndGroundedChecks();

        //check inputs every frame
        CheckInputs();

        //update the visual indicator of the grenade trajectory
        DrawThrowableLine();
    }
}

