using UnityEngine;
using System.Collections;

public class sideWalls: MonoBehaviour
{

    void OnTriggerEnter(Collider hitInfo)
    {
        if (hitInfo.name == "Ball")
        {
            string wallName = transform.name;
            GameManager.Score(wallName);
            hitInfo.gameObject.SendMessage("RestartGame", 1.0f, SendMessageOptions.RequireReceiver);

            Debug.Log("OUT");
        }
    }
}