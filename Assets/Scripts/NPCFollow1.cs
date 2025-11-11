using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    public Transform followplayer; // Reference to the player's transform
    public float distanceFromChar;
    public List<Vector2> followCharPos = new List<Vector2>();
    public float allowableDistance;
    public float sampleTimediff;
    float sampleTime;
    public float followspeed;
    public float fastMarch;
    public float normalSpeed;
    public float fastDistance;
    //private RigidBody2D rb;
    //private Animator anim;
    // Start is called before the first frame update
    void Start()
    {

        sampleTime = Time.time;
        followCharPos.Add(followplayer.position);
        followspeed = fastMarch;
        // rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > sampleTime)
        {
            sampleTime = Time.time + sampleTimediff;
            if (Vector2.Distance(transform.position, followplayer.position) > distanceFromChar
            && Vector2.Distance(followplayer.position, followCharPos[followCharPos.Count - 1]) > allowableDistance)
            {
                followCharPos.Add(followplayer.position);
            }

        }
        if (Vector2.Distance(transform.position, followplayer.position) > fastDistance)
        {
            followspeed = fastMarch;
        }
        else
        {
            followspeed = normalSpeed;
        }

        if (Vector2.Distance(transform.position, followplayer.position) > distanceFromChar)
        {
            //Vector2 dir = (followCharPos[0] - (Vector2)transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, followCharPos[0], Time.deltaTime * followspeed);
        }
        if (Vector2.Distance(transform.position, followCharPos[0]) < 0.05f)//0.05 remove dist variable
        {
            if (followCharPos.Count > 1) followCharPos.RemoveAt(0);
        }
    }
}
