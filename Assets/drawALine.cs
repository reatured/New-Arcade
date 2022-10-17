using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawALine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject starting, ending; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pos = new Vector3[2];
        Vector3 pos1 = starting.transform.position;
        Vector3 pos2 = ending.transform.position;

        pos[0] = pos1;
        pos[1] = pos2;
        lineRenderer.SetPositions(pos);
    }
}
