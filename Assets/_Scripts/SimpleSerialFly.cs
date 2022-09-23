using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SimpleSerialFly : MonoBehaviour
{

    public String portName = "COM5";  // use the port name for your Arduino, such as /dev/tty.usbmodem1411 for Mac or COM3 for PC
    public Animator flyTrap;
    public Transform fly;
    public FlyAI FlyScript;
    public Transform referencePoint; //reference point on flytrap for distance calculation
    public ParticleSystem efx;
    public AudioSource audioSource;
    public AudioClip slapSound;

    private SerialPort serialPort = null; 
    private int baudRate = 115200;  // match your rate from your serial in Arduino
    private int readTimeOut = 100;

    private string serialInput;

    bool programActive = true;
    Thread thread;

    bool close = false;

    void Start()
    {
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
        
        //stop the animation from playing setting the speed = 0; 
        flyTrap.speed = 0f;
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

    void Update()
    {
        if (serialInput != null)
        {
            string[] strEul = serialInput.Split(';');  // parses using semicolon ; into a string array called strEul. I originally was sending Euler angles for gyroscopes
            if (strEul.Length == 2) // only uses the parsed data if every input expected has been received. In this case, 2 inputs consisting of a button (0 or 1) and an analog values between 0 and 1023
            {
                float readout = float.Parse(strEul[0]);
                readout = Mathf.Clamp(readout, 200f, 824f);
                readout = map(readout, 200f, 824f, -0f, 1f);
               
                flyTrap.Play("modifiedAnimation", 0, readout);

                //see if fly is in the range
                float dist = Vector3.Distance(fly.position, referencePoint.position);
                //if the flytrap is close turn on 
                if (readout > 0.65f && !close)
                {
                    if (dist < 0.66f)
                    {
                        //create the particle effect
                        Instantiate(efx, referencePoint.position, Quaternion.identity);
                        FlyScript.reset();

                        //play sound effect
                        audioSource.clip = slapSound;
                        audioSource.Play();

                        //fly is gameover
                        Debug.Log("HIT!!");
                    }
                    close = true;
                }
                if (readout < 0.65f)
                {

                    close = false;
                }

                //Debug.Log(dist);
            }
        }
    }

    public static float map(float value, float leftMin, float leftMax, float rightMin, float rightMax)
    {
        return rightMin + (value - leftMin) * (rightMax - rightMin) / (leftMax - leftMin);
    }

    public void OnDisable()  // attempts to closes serial port when the gameobject script is on goes away
    {
        programActive = false;
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}