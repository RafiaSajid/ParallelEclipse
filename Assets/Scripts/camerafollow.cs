// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class camerafollow : MonoBehaviour
// {
//    public float followSpeed = 2f;
//    public float yOffset = 6.5f,  xOffset = -15f;
//    public Transform target;
//     // Update is called once per frame
//     void Update()
//     {   if(target.position.x >= -33.79907){
//        Vector3 newpos = new Vector3(target.position.x - xOffset, target.position.y + yOffset, -10f);
//         transform.position  = Vector3.Slerp(transform.position,newpos,followSpeed* Time.deltaTime);
//     }
//     else{
//          Vector3 newpos = new Vector3(target.position.x + xOffset, target.position.y + yOffset, -10f);
//         transform.position  = Vector3.Slerp(transform.position,newpos,followSpeed* Time.deltaTime);

//     }

//     }
// }
using UnityEngine;

public class camerafollow : MonoBehaviour
{
    [SerializeField] private Transform target; //player transform
    //[SerializeField] private float followSpeed = 6f;
    [SerializeField] private float xOffset = 23.7f;
    [SerializeField] private float yOffset = 6.5f;

    // Camera limits 
    [SerializeField] private float minX = -9f; // leftmost boundary
    [SerializeField] private float maxX = 50f;   // rightmost boundary
    [SerializeField] private float minY = 0f;     // bottom limit
    [SerializeField] private float maxY = 20f;    // top limit

    [SerializeField] private Vector3 velocity = Vector3.zero;
    [SerializeField] private float smoothTime = 0.3f;
    void LateUpdate() // unity callback after all Update calls
    { // target has moved then camera moves
        if (target == null)
        {
            Debug.Log("No target assigned");
            return;
        }
        // Calculate desired position
        Vector3 desiredPosition = new Vector3(target.position.x - xOffset, target.position.y + yOffset, -10f);
        //compute where camera should be relative to target,
        // including X/Y offsets. z = -10f sets a typical orthographic camera depth.


        // Clamp to prevent showing background
        //clamp better than if/else for boundaries
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX); // (value within bound, min, max)
        float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // Mathf.Clamp :restricts a given value to a specified range between a minimum and a maximum
        //no out of bounds camera movement.
        // Vector3 smoothPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, -10f),
        //  followSpeed * Time.deltaTime);
        //vector3.Lerp (current position, target position, interpolation factor)

        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, new Vector3(clampedX, clampedY, -10f),
            ref velocity, smoothTime);

        //The function calculates a new Vector3 that lies on
        //  the straight line connecting current and target.
        transform.position = smoothPosition; // position direct changes 
    }
}


//NOTES
/*
Why use Lerp instead of MoveTowards?
A: Lerp provides linear smooth interpolation between positions;

SmoothDamp is specifically designed to smoothly track a target that 
is constantly moving. It dynamically adjusts its speed based on 
how fast the target is moving away or towards it.

 .*/

