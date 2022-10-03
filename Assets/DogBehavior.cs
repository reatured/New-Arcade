using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dogWinText;
    public GameObject dogLoseText;
    public Scrollbar dogHealth;

    public float healthTime = 10f; 
    private float startingTime = 0f; 


    void Start()
    {
        dogHealth.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {

        dogHealth.value = 1 - ((Time.time - startingTime) / healthTime); 
        if(dogHealth.value <= 0)
        {
            dogHealth.value = 0f;
            dogLoseText.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        dogWinText.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void startTimer()
    {
        startingTime = Time.time;
        Debug.Log(startingTime);
        dogWinText.SetActive(false);
        dogLoseText.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
