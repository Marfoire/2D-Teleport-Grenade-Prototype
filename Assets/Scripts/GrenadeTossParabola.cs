using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeTossParabola : MonoBehaviour
{

    public Transform someObject; //object that moves along parabola.
    float objectT = 0; //timer for that object

    public float h; //desired parabola height

    Vector3 a, b; //Vector positions for start and end

    public PlayerController pScript;

    public bool breakParabola;

    float tempTime = 0;

    Vector2 lastRBPosition;
    Vector2 firstRBPosition;

    Vector3 result;

    public float gravValue;
    public float gravBaseVelocity;
    public float accelValue;
    public Vector3 travelDirection;

    public Vector3 up;

    public int parabolaCallLoopCount;


    public void GetInfo(PlayerController ps)
    {
        pScript = ps;
        a = pScript.rb.position;
        b = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        h = pScript.throwHeight;
        someObject = transform;
        Physics2D.IgnoreCollision(pScript.bc, GetComponent<CircleCollider2D>());
    } 

    ///////////////////////////FOR TROUBLESHOOTING
    /*void OnDrawGizmos()
    {
        //Draw the height in the viewport, so i can make a better gif :]
       // Handles.BeginGUI();
       // GUI.skin.box.fontSize = 16;
       // GUI.Box(new Rect(10, 10, 100, 25), h + "");
       // Handles.EndGUI();

        //Draw the parabola by sample a few times
        Gizmos.color = Color.red;
        Gizmos.DrawLine(a, b);
        float count = 20;
        Vector3 lastP = a;
        for (float i = 0; i < count + 1; i++)
        {
            Vector3 p = SampleParabola(a, b, h, i / count);
            Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
            Gizmos.DrawLine(lastP, p);
            lastP = p;
        }
    }*/


    Vector3 LaunchParabola(Vector3 startPoint, Vector3 endPoint, float height, float t)
    {
        parabolaCallLoopCount++;
        lastRBPosition = new Vector2(result.x, result.y);
        if (lastRBPosition.y == 0 && lastRBPosition.x == 0)
        {
            lastRBPosition = GetComponent<Rigidbody2D>().position;
        }
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(startPoint.y - endPoint.y) < 20)
        {
            //start and end are roughly level
            travelDirection = endPoint - startPoint;
            result = startPoint + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            if (parabolaCallLoopCount == 2)
            {
                gravValue = (result.y - lastRBPosition.y) - (lastRBPosition.y - startPoint.y);
            }
            return result;
        }
        else
        {
            //start and end are not level
            travelDirection = endPoint - startPoint;
            Vector3 levelDirection = endPoint - new Vector3(startPoint.x, endPoint.y, startPoint.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirection);
            up = Vector3.Cross(right, levelDirection);
            if (endPoint.y > startPoint.y) up = -up;
            result = startPoint + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            if (parabolaCallLoopCount == 2)
            {
                gravValue = (result.y - lastRBPosition.y) - (lastRBPosition.y - startPoint.y)/Time.fixedDeltaTime;
            }
            return result;
        }
    }

    private void UpdateParabolaProperties()
    {
        if (someObject && breakParabola == false)
        {
            
            tempTime += Time.fixedDeltaTime;

            objectT = Mathf.Clamp(tempTime, 0, 1); 

            if (objectT < 1)
            {
                GetComponent<Rigidbody2D>().position = LaunchParabola(a, b, h, objectT);
            }
            else if (breakParabola == false)
            {
                breakParabola = true;
                gravBaseVelocity = (GetComponent<Rigidbody2D>().position.y - lastRBPosition.y) / Time.fixedDeltaTime;
                accelValue = (GetComponent<Rigidbody2D>().position.x - lastRBPosition.x) / Time.fixedDeltaTime;
                GetComponent<Rigidbody2D>().velocity = new Vector2(accelValue, gravBaseVelocity);
            }

        }
        else if (breakParabola == true)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x - ((accelValue / (GetComponent<Rigidbody2D>().velocity.x)) * Time.fixedDeltaTime), (GetComponent<Rigidbody2D>().velocity.y + (gravValue /** 2*/ * Time.fixedDeltaTime)));
        }
    }

    private void FixedUpdate()
    {
        UpdateParabolaProperties();
    }

}
