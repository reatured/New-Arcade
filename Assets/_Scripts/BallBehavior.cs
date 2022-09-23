using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private Rigidbody rb;
    AudioSource audioData;

    public float speed = 200f;
    // Start is called before the first frame update
    void Start()
    {
        audioData = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        Invoke("GoBall", 2);
    }

    //GoBall will choose a random direction (left or right), then make the ball start to move.
    void GoBall()
    {
        // Flip a coin to determine if the ball starts left or right
        float x = Random.value < 0.5f ? -1f : 1f;

        // Flip a coin to determine if the ball goes up or down. Set the range
        // between 0.5 -> 1.0 to ensure it does not move completely horizontal.
        float y = Random.value < 0.5f ? Random.Range(-1f, -0.5f)
                                      : Random.Range(0.5f, 1f);

        Vector3 direction = new Vector3(x, y, 0);
        GetComponent<Rigidbody>().AddForce(direction * speed);
    }

    //when a win condition is met. It stops the ball, and resets its position to the center of the board.
    void ResetBall()
    {
        rb.velocity = Vector3.zero;
        transform.position =new  Vector3(0,0,10);
    }

    //when our restart button is pushed. We’ll add that button later.
    void RestartGame()
    {
        Debug.Log("Restarting");
        ResetBall();
        Invoke("GoBall", 1);
    }

    //waits until we collide with a paddle.
    void OnCollisionEnter(Collision coll)
    {
        audioData.Play();
        if (coll.collider.CompareTag("Player"))
        {

            Vector3 normal = coll.GetContact(0).normal;
            rb.AddForce(-normal*1.2f, ForceMode.Impulse);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
