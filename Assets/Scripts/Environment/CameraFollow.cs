using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target; //what the camera follows
    private Transform targetPosition; //the position of the target
    private Vector3 startPos; //starting position of the camera screen
    public float camSpeed = 3; //how fast the cam moves

    void Start()
    {
        //get initial start pos for camera
        startPos = transform.position;

        //get position of the target (player)
        targetPosition = target.transform;
    }

    void LateUpdate()
    {
        //when the player passes the camera's starting location, it starts following the player
        if (transform.position.x >= startPos.x)
        {
            //how fast the camera lerps
            float i = camSpeed * Time.deltaTime;
            
            //moving the camera
            //clamp stops it from moving past the initial starting screen
            //lerp smooths camera moving
            transform.position = new Vector3(Mathf.Clamp(Mathf.Lerp(this.transform.position.x, targetPosition.transform.position.x, i), startPos.x, Mathf.Lerp(this.transform.position.x, targetPosition.transform.position.x, i)), transform.position.y, targetPosition.position.z);
        }
    }

}
