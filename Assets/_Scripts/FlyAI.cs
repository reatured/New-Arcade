using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAI : MonoBehaviour
{
    private float radius = 6.5f;
    private float currentAngle = 0f;
    private float angleSpeed = 0.003f;

    // Range over which height varies.
    float heightScale = 1.0f;
    // Distance covered per second along X axis of Perlin plane.
    float xScale = 1.0f;

    private Vector3 center;
    // Start is called before the first frame update
    void Start()
    {
        center = this.transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        float height = heightScale * Mathf.PerlinNoise(Time.time * xScale, 0.0f);
        float arm = radius*0.5f + radius*0.5f*height;

        currentAngle += angleSpeed;
        float posZ = Mathf.Cos(currentAngle) * arm;
        float posY = Mathf.Sin(currentAngle) * arm;

        this.transform.position = new Vector3(0, posY, posZ) + center;
    }

    public void reset() {
        currentAngle = 0f;
    }
}
