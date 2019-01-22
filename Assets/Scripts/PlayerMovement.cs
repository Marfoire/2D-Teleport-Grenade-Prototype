using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //horizontal movement variables
    public float storedLastInput;
    public float MaxSpeed;
    public float AccelerationTime;
    public float DecelerationTime;
    public float totalVelocity;
    public float deltaVelocity;
    public float prevVelocity;

    //jump variables
    public float gravityValue;
    public float jumpHeight;
    public float jumpTime;
    public float jumpVelocity;
    public float terminalVelocity;
    public bool grounded;

    //reference variables
    public Rigidbody2D rb;

    //the raycast for the grouned checks
    RaycastHit2D hit;
    //RaycastHit2D clipHit;

    public BoxCollider2D bc;

    public bool clingingToWall;
    public float clingBufferStart;
    public bool bufferedCling;

    public bool jumpInputted;
    public bool leftInputted;
    public bool rightInputted;

    public bool isJumping;
    bool doubleJump;
    RaycastHit2D leftWallCheck;
    RaycastHit2D rightWallCheck;

    public GameObject nadePrefab;

    public bool preventNade;

    // Use this for initialization
    void Start()
    {
        //get references
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        //use motion equations to get gravity and jump velocity
        gravityValue = (terminalVelocity / 0.3333f);
        jumpVelocity = -(gravityValue * jumpTime);
    }

    //method for jumping
    void JumpCall()
    {

        if (isJumping && jumpInputted && clingingToWall == false)
        {
            rb.velocity = Vector2.zero;
            isJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpVelocity);
            doubleJump = true;
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
            //rb.MovePosition(new Vector2(rb.position.x, rb.position.y + jumpVelocity));
        }


        if (jumpInputted == true && clingingToWall == true)
        {

            if ((totalVelocity * Input.GetAxis("Horizontal") > 0 && leftInputted != true) && bufferedCling == false || (totalVelocity * Input.GetAxis("Horizontal") < 0 && rightInputted != true) && bufferedCling == false)
            {
                totalVelocity = -totalVelocity * 5;
                prevVelocity = -prevVelocity * 5;
            }
            else if ((totalVelocity * Input.GetAxis("Horizontal") > 0 && leftInputted == true))
            {
                storedLastInput = -1;
            }
            else if ((totalVelocity * Input.GetAxis("Horizontal") < 0 && rightInputted == true))
            {
                storedLastInput = 1;
            }
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpVelocity);
            isJumping = true;
            bufferedCling = false;
            clingingToWall = false;
            clingBufferStart = 0;
        }
    }

    //recycled from shipmotor
    public void HorizontalMovement()
    {
        //if the either of the input axis are receiving RAW input, start running some acceleration calculations (GetAxisRaw only returns 0,1,-1 based on what input is pressed)
        if (leftInputted == true || rightInputted == true)
        {

            storedLastInput = Input.GetAxisRaw("Horizontal");//store the input values in a vector2 for deceleration

            leftWallCheck = Physics2D.Raycast(new Vector2((bc.bounds.center.x) - (bc.bounds.extents.x + 0.1f), bc.bounds.center.y - (bc.bounds.extents.y - 0.1f)), Vector2.up, (bc.bounds.extents.y * 2 - 0.8f));
            rightWallCheck = Physics2D.Raycast(new Vector2((bc.bounds.center.x) + (bc.bounds.extents.x + 0.1f), bc.bounds.center.y - (bc.bounds.extents.y - 0.1f)), Vector2.up, (bc.bounds.extents.y * 2 - 0.8f));


            //If the acceleration time is not 0
            if (AccelerationTime != 0 && ((leftWallCheck == true && rightInputted == true) || (rightWallCheck == true && leftInputted == true) || (leftWallCheck == false && rightWallCheck == false)))
            {
                deltaVelocity = MaxSpeed / AccelerationTime * Time.fixedDeltaTime;//find acceleration delta velocity with maxSpeed divided by acceleration time
                totalVelocity = prevVelocity + deltaVelocity;//add the delta velocity to the previous velocity to get the current total velocity                
            }
            else if(AccelerationTime == 0 && ((leftWallCheck == true && rightInputted == true) || (rightWallCheck == true && leftInputted == true) || (leftWallCheck == false && rightWallCheck == false)))  //if the acceleration time is 0, don't worry about doing acceleration calculations
            {
                totalVelocity = MaxSpeed;//set the velocity to the max speed
            }

            //totalStart++; //this is for testing
        }

        //if there is no input being detected and the velocity isn't 0
        else if (leftInputted == false && rightInputted == false && totalVelocity != 0)
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
            //totalEnd++; //this is for testing
        }

        //run a check to see if the velocity is greater than the speed cap
        if (totalVelocity > MaxSpeed)
        {
            totalVelocity = MaxSpeed;//correct the velocity back to the max speed
            //print("Acceleration time in frames: " + totalStart);
        }

        //if total velocity rolls over to be negative when decelerating
        if (totalVelocity < 0)
        {
            storedLastInput = 0;//reset the input to 0
            totalVelocity = 0;//set the total velocity back to 0 to fix that
            //print("Deceleration time in frames: " + totalEnd);
        }

        //set the previous velocity to the current total velocity for the next loop around with this function
        prevVelocity = totalVelocity;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCorrectPosition(collision);
    }

    public void TryCorrectPosition(Collision2D incomingCollider)
    {
        
        /*if (incomingCollider == null)
        {
            clipHit = Physics2D.Raycast(new Vector2(0.1f + bc.bounds.center.x - (bc.bounds.extents.x), rb.position.y - (bc.bounds.extents.y + 0.1f)), Vector2.right, bc.bounds.extents.x * 2 - 0.1f);
        }*/
        if (incomingCollider != null)
        {
            ColliderDistance2D stageCheck = bc.Distance(incomingCollider.collider);
            if (stageCheck.isOverlapped && stageCheck.isValid)
            {
                Debug.DrawLine(stageCheck.pointB, stageCheck.pointA, Color.cyan);
                Vector2 correction = (stageCheck.normal * stageCheck.distance);
                rb.position = rb.position + correction;
                transform.position = rb.position;
                //print(correction);
            }
        }
    }



    //checkGrounded does some raycasting and position correcting for the purpose of keeping thje player grounded
    void CheckGrounded()
    {
        if (hit.collider != null)
        {//if the ray collider with something

            if (hit.collider.bounds.center.y + hit.collider.bounds.extents.y - 1 < rb.position.y - bc.bounds.extents.y && hit.collider.gameObject.tag == "Stage")//if the ray is colliding with the topside of the stage piece it connected with
            {
                grounded = true;//set grounded to true
                doubleJump = false;
                isJumping = false;
            }
            }
        else//if there are no collisions
        {
            grounded = false;//set grounded to false

        }
    }

    public void DrawThrowableLine()
    {
        float count = 20;
        GetComponent<LineRenderer>().positionCount = (int)count + 1;
        for (float i = 0; i < count + 1; i++)
        {
            Vector3 p = MapParabola(rb.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), 150, i / count);
            GetComponent<LineRenderer>().SetPosition((int)i, new Vector3(p.x, p.y, -1));
        }
    }

    Vector3 MapParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 20)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirection = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirection);
            Vector3 up = Vector3.Cross(right, levelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }



    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && preventNade == false)
        {
            preventNade = true;
            GameObject nadeObject = Instantiate(nadePrefab, transform.position, Quaternion.identity);
           // nadeObject.GetComponent<Nade>().GetTrajectoryInformation(Camera.main.ScreenToWorldPoint(Input.mousePosition), this);
            nadeObject.GetComponent<GrenadeScript>().GetInfo(this);
        }

        if (Input.GetButtonDown("Jump") == true)
        {
            jumpInputted = true;
        }

        if (Input.GetKey("left") == true || Input.GetKey("a"))
        {
            leftInputted = true;
        }

        if (Input.GetKey("right") == true || Input.GetKey("d"))
        {
            rightInputted = true;
        }

        Debug.DrawRay(new Vector2(0.1f + bc.bounds.center.x - (bc.bounds.extents.x), rb.position.y - (bc.bounds.extents.y + 0.1f)), Vector2.right * (bc.bounds.extents.x * 2 - 0.1f), Color.white);

        hit = Physics2D.Raycast(new Vector2(0.1f + bc.bounds.center.x - (bc.bounds.extents.x), rb.position.y - (bc.bounds.extents.y + 0.1f)), Vector2.right, bc.bounds.extents.x * 2 - 0.1f);


        Debug.DrawRay(new Vector2((rb.position.x) - (bc.bounds.extents.x + 0.1f), rb.position.y - (bc.bounds.extents.y - 0.1f)), Vector2.up * ((bc.bounds.extents.y * 2 - 0.8f)), Color.yellow);
        Debug.DrawRay(new Vector2((rb.position.x) + (bc.bounds.extents.x + 0.1f), rb.position.y - (bc.bounds.extents.y - 0.1f)), Vector2.up * ((bc.bounds.extents.y * 2 - 0.8f)), Color.yellow);

        leftWallCheck = Physics2D.Raycast(new Vector2((bc.bounds.center.x) - (bc.bounds.extents.x + 0.1f), bc.bounds.center.y - (bc.bounds.extents.y - 0.1f)), Vector2.up, (bc.bounds.extents.y * 2 - 0.8f));
        rightWallCheck = Physics2D.Raycast(new Vector2((bc.bounds.center.x) + (bc.bounds.extents.x + 0.1f), bc.bounds.center.y - (bc.bounds.extents.y - 0.1f)), Vector2.up, (bc.bounds.extents.y * 2 - 0.8f));


        DrawThrowableLine();
    }

    void FixedUpdate()
    {


        CheckGrounded();

        //call horizontal movement
        HorizontalMovement();

        //check the jump
        JumpCall();


        rb.velocity = new Vector2((totalVelocity * storedLastInput), rb.velocity.y);
        //if the player is not grounded
        if (grounded == false)
        {
            //clamp the velocity between the terminal velocity and the intial velocity
            Mathf.Clamp(rb.velocity.y, terminalVelocity, jumpVelocity);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + (gravityValue * Time.fixedDeltaTime));
        }

        leftInputted = false;
        rightInputted = false;
        jumpInputted = false;
    }
}

