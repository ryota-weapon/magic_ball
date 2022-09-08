using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    GameObject Ball;
    Vector3 myPos;
    float distance_to_ball = 3.0f;

    
    void Start()
    {
        
    }


    void Update()
    {
     
        if(Ball != null)
        {
            if (this.transform.position.x < -30f)
            {
                return;
            }
            myPos = this.transform.position;
            myPos.x = Ball.transform.position.x + distance_to_ball;
            myPos.z = Ball.transform.position.z;
            this.transform.position = myPos;

        }


    }

    public void FindBallAndFollowIt()
    {
        Ball = GameObject.FindWithTag("Ball");

    }
}
