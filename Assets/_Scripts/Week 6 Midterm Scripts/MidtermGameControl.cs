using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
//using System.Numerics;

public class MidtermGameControl : MonoBehaviour
{
    public String portName = "COM15";  // use the port name for your Arduino, such as /dev/tty.usbmodem1411 for Mac or COM3 for PC

    private SerialPort serialPort = null;
    private int baudRate = 115200;  // match your rate from your serial in Arduino
    private int readTimeOut = 100;

    private string serialInput;

    bool programActive = true;
    Thread thread;
    //variables for arduino

    //my variable:
    public GameObject tree;
    public float xMin, xMax, zMin, zMax;
    public Vector3 rocketOriginalPosition; 
    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, -1, 0);
        try
        {
            serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.ReadTimeout = readTimeOut;
            serialPort.Open();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        thread = new Thread(new ThreadStart(ProcessData));  // serial events are now handled in a separate thread
        thread.Start();
        //arduino script above

        //my script:
        for(int i = 0; i < 300; i++)
        {
            float x = UnityEngine.Random.Range(xMin, xMax);
            float z = UnityEngine.Random.Range(zMin, zMax);
            GameObject ttree = Instantiate(tree, new Vector3(x, -0.441f, z), Quaternion.identity,this.transform);

            float scale = UnityEngine.Random.Range(0.8f, 1.2f);
            ttree.transform.localScale *= scale; 
        }

        rocketOriginalPosition = rocket.gameObject.transform.position;
        gameOverText.gameObject.SetActive(false);
    }

    void ProcessData()
    {
        Debug.Log("Thread: Start");
        while (programActive)
        {
            try
            {
                serialInput = serialPort.ReadLine();
            }
            catch (TimeoutException)
            {

            }
        }
        Debug.Log("Thread: Stop");
    }

    public float frogReading, owlReading;
    public bool launchButton = false;
    public bool launchState = false; 
    public Slider frogSlider, owlSlider;
    public UnityEvent<Vector3> launch;
    public UnityEvent<Vector3> addTorque;
    [Space(10)]
    public float frogMagnifier;
    [Range(0f, 10f)]
    public float owlMagnifier; 
    public Rigidbody rocket; 

    public bool owlBreakRight = false, owlBreakLeft = false;

    public bool frogBreakRight = false, frogBreakLeft = false; 
    

    public TextMeshProUGUI impactSpeedText;
    public float impactSpeed;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (serialInput != null)
        {
            string[] strEul = serialInput.Split(';');  // parses using semicolon ; into a string array called strEul. I originally was sending Euler angles for gyroscopes
            float readout = float.Parse(strEul[0]);
            frogReading = readout;

            readout = float.Parse(strEul[1]);
            owlReading = readout;

            launchButton = float.Parse(strEul[2]) == 1? false:true;
            

            frogSlider.value = map(frogReading, 0, 1023, 1, 0);
            Vector3 launchPulse = new Vector3(0, frogSlider.value * frogMagnifier, 0);  
            owlSlider.value = map(owlReading, 0, 1023, 1, 0);
            Vector3 angularPulse = new Vector3(0, 0, -owlSlider.value * owlMagnifier);

            if (launchButton)
            {
                if (!launchState)
                {

                    launch.Invoke(launchPulse);
                }
                

                launchState = true;

            }

            if (launchState == true)
            {
                if(owlReading > 800)
                {
                    if (!owlBreakRight)
                    {
                        owlBreakRight = true;
                        owlBreakLeft = false;
                        addTorque.Invoke(angularPulse);
                    }
                    
                }
                else if(owlReading < 200)
                {
                    if (!owlBreakLeft)
                    {
                        owlBreakLeft = true;
                        owlBreakRight = false;
                        addTorque.Invoke(angularPulse);
                    }
                    
                }

                if (frogReading > 800)
                {
                    if (!frogBreakRight)
                    {
                        frogBreakRight = true;
                        frogBreakLeft = false;
                        launch.Invoke(new Vector3(0, frogSlider.value * frogMagnifier, 0));
                    }
                    
                }
                else if (frogReading < 200)
                {
                    if (!frogBreakLeft)
                    {
                        frogBreakLeft = true;
                        frogBreakRight = false;
                        launch.Invoke(new Vector3(0, frogSlider.value * frogMagnifier, 0));
                    }
                    
                }
            }
            if(blowed == false)
            {
                impactSpeed = rocket.velocity.magnitude;
                impactSpeedText.text = "Impact Speed: " + impactSpeed.ToString("F2");
            }
            
        }
    }
    
    public static float map(float value, float leftMin, float leftMax, float rightMin, float rightMax)
    {
        return rightMin + (value - leftMin) * (rightMax - rightMin) / (leftMax - leftMin);
    }

    public Vector3 randomVec3(float min, float max)
    {
        Vector3 result = new Vector3(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));

        return result;
    }

    //public void restart()
    //{
    //    rocket.gameObject.transform.position = new Vector3()
    //}

    public bool blowed = false; 
    public void reload()
    {
        rocket.gameObject.transform.position = rocketOriginalPosition;
        rocket.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        rocket.velocity = Vector3.zero; 
        rocket.angularVelocity = Vector3.zero;  

    }
    public float count = 0;
    public TextMeshProUGUI gameOverText;
    public float smallestTest = 100; 
    public void hitAndExplode()
    {
        blowed = true; 
        float treeCount = this.transform.childCount;
        float RadiusMaximun = Mathf.Min((xMax - xMin), (zMax - zMin));
        float explodeRadius = map(impactSpeed, 0, 10, 1, RadiusMaximun);

        GameObject explosion = rocket.gameObject.transform.GetChild(0).gameObject;
        
        explosion.transform.localScale *= explodeRadius;
        explosion.SetActive(true);


        for (int i = 0; i < treeCount; i++)
        {
            GameObject targetTree = this.transform.GetChild(i).gameObject;
            float distance = (targetTree.transform.position - rocket.position).magnitude;
            
            if(distance < explodeRadius)
            {
                if(distance < smallestTest)
                {
                    smallestTest = distance;
                }
                count++; 
                targetTree.transform.GetChild(0).gameObject.SetActive(true);
            }

            if(count > 1)
            {
                gameOverText.text = count + " Trees Destroyed!!!!";
            }else if(count == 1)
            {
                gameOverText.text =  "One Tree Destroyed!!!!";
            }else if(count == 0)
            {
                
            }

            gameOverText.gameObject.SetActive(true); 
        }
    }

    public void hitTheGround()
    {
        blowed = true; 
        GameObject explosion = rocket.gameObject.transform.GetChild(0).gameObject;
        explosion.SetActive(true);
        gameOverText.text = "You missed everything, sucker";
        gameOverText.gameObject.SetActive(true);
    }
}
