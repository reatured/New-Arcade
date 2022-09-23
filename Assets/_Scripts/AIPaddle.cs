﻿using UnityEngine;

public class AIPaddle : MonoBehaviour
{
    public Rigidbody ball;
    public float speed = 80f;

    private void FixedUpdate()
    {
        // Check if the ball is moving towards the paddle (positive x velocity)
        // or away from the paddle (negative x velocity)
        if (ball.velocity.x > 0f)
        {
            // Move the paddle in the direction of the ball to track it
            if (ball.position.y > GetComponent<Rigidbody>().position.y)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * speed);
            }
            else if (ball.position.y < GetComponent<Rigidbody>().position.y)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.down * speed);
            }
        }
        else
        {
            // Move towards the center of the field and idle there until the
            // ball starts coming towards the paddle again
            if (GetComponent<Rigidbody>().position.y > 0f)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.down * speed);
            }
            else if (GetComponent<Rigidbody>().position.y < 0f)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * speed);
            }
        }
    }

}
