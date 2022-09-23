using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class topDownWalls : MonoBehaviour
{
    public float bounceStrength = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        BallBehavior ball = collision.gameObject.GetComponent<BallBehavior>();

        if (ball != null)
        {
            // Apply a force to the ball in the opposite direction of the
            // surface to make it bounce off
            Vector3 normal = collision.GetContact(0).normal;
            ball.GetComponent<Rigidbody>().AddForce(-normal * bounceStrength, ForceMode.Impulse);
        }
    }

}
