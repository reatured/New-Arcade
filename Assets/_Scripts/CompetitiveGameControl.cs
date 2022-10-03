using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;
public class CompetitiveGameControl : MonoBehaviour
{

    public String portName = "COM15";  // use the port name for your Arduino, such as /dev/tty.usbmodem1411 for Mac or COM3 for PC

    private SerialPort serialPort = null;
    private int baudRate = 115200;  // match your rate from your serial in Arduino
    private int readTimeOut = 100;

    private string serialInput;

    bool programActive = true;
    Thread thread;

    public GameObject dog;
    public GameObject man;

    public Scrollbar hunterHealth;
    void Start()
    {
        hunterHealth.value = 1f; 


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
    float verticalMovement;
    float horizontalMovement;
    float buttonPushed;
    float rotateRight;
    float rotateUp;
    public float speed = 1;
    public float forwardSpeed = 10;

    private Quaternion from = Quaternion.Euler(0, 0, 0);
    private Quaternion to = Quaternion.Euler(0, 180, 0);
    public float rotateTime = 1;
    private float journeyTime = 0f;
    private float startTime = 100f; 
    public bool currentState = false; //not turned back

    public float hunterHealthMax = 5;
    private float hunterHealthCurrent = 5;
    public GameObject hunterLoseText;
    public GameObject hunterWinText;
    void FixedUpdate()
    {

        hunterHealth.value = hunterHealthCurrent / hunterHealthMax;
        if (hunterHealthCurrent == 0)
        {
            hunterLoseText.SetActive(true);
            Time.timeScale = 0.0f;
        }
        if (serialInput != null)
        {
            string[] strEul = serialInput.Split(';');  // parses using semicolon ; into a string array called strEul. I originally was sending Euler angles for gyroscopes

            float readout = float.Parse(strEul[0]);
            if (readout < 400 || readout > 650)
            {
                readout = -map(readout, 0f, 1023f, -4f, 4f);
                verticalMovement = readout;
            }
            else
            {
                verticalMovement = 0f;
            }




            readout = float.Parse(strEul[2]);
            readout = map(readout, 0f, 1023f, -4f, 4f);
            horizontalMovement = readout * -1 * Time.deltaTime * speed;
            rotateRight = map(readout, -4f, 4f, -45f, 45f) * Time.deltaTime * speed;

            readout = float.Parse(strEul[4]);
            buttonPushed = (int)readout;

            journeyTime = Time.time - startTime;

            if (buttonPushed == 1) //turn back
            {
                if (currentState == false)
                {

                    currentState = true;
                    startTime = Time.time;

                    hunterHealthCurrent--;

                }
            }

            if (journeyTime < rotateTime)
            {
                man.transform.rotation = Quaternion.Lerp(from, to, journeyTime / rotateTime);

            }
            else if (journeyTime >= rotateTime && journeyTime < rotateTime + 1)
            {

                if(verticalMovement != 0f)
                {
                    hunterWinText.SetActive(true);
                    Time.timeScale = 0.0f;
                }
            }
            else if (journeyTime > rotateTime + 1 && journeyTime < rotateTime + 1 + rotateTime)
            {

                man.transform.rotation = Quaternion.Lerp(from, to, 1 - (journeyTime - 1 - rotateTime) / rotateTime);
            }
            else if (journeyTime > rotateTime + 1 + rotateTime)
            {

                currentState = false;
            }





            dog.transform.Translate(new Vector3(0, 0, verticalMovement * Time.deltaTime));

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

    public void startGame()
    {
        hunterHealthCurrent = hunterHealthMax;
        hunterWinText.SetActive(false);
        hunterLoseText.SetActive(false);
    }
}