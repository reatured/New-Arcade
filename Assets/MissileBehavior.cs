using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    [Range(0f, 1f)]
    public float torqueResistence = 0.5f;
    public Animator frogAnim, owlAnim;

    public MidtermGameControl gameControl; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(GetComponent<Rigidbody>().angularVelocity, Vector3.zero, torqueResistence);
    }

    public void launch(Vector3 launchForce)
    {
        frogAnim.SetTrigger("Jump");
        GetComponent<Rigidbody>().AddRelativeForce(launchForce);
    }

    public void addAngularVel(Vector3 angularForce)
    {
        owlAnim.SetTrigger("Fly");
        GetComponent<Rigidbody>().AddRelativeTorque(angularForce);  
    }

    public CameraMovement cameraFollow;
    public Transform treeTrans;
    public AudioSource explosion; 
    private void OnTriggerEnter(Collider other)
    {

        if(other.name == "Ground" && gameControl.launchState)
        {
            explosion.Play();
            GetComponent<Rigidbody>().isKinematic = true;
            gameControl.hitTheGround();
            
        }

        if(other.CompareTag("Tree"))
        {
            explosion.Play();
            cameraFollow.rocket = treeTrans; 
            gameControl.hitAndExplode();

            GetComponent<Rigidbody>().isKinematic = true;
            Destroy(this);
        }
    }
}
