using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform rocket;
    public Vector3 offset; 
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - rocket.position; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = rocket.position + offset;
        this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, 0.2f);
    }
}
