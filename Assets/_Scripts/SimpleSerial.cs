using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SimpleSerial : MonoBehaviour
{

    public String portName = "COM15";  // use the port name for your Arduino, such as /dev/tty.usbmodem1411 for Mac or COM3 for PC

    private SerialPort serialPort = null; 
    private int baudRate = 115200;  // match your rate from your serial in Arduino
    private int readTimeOut = 100;

    private string serialInput;

    bool programActive = true;
    Thread thread;

    public GameObject aircraft;
    public Vector3 airPos; 
    void Start()
    {
        

        aircraft.transform.position = airPos; 
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
    float rotateRight;
    float rotateUp; 
    public float speed = 1;
    public float forwardSpeed = 10; 
    void Update()
    {
        if (serialInput != null)
        {
            string[] strEul = serialInput.Split(';');  // parses using semicolon ; into a string array called strEul. I originally was sending Euler angles for gyroscopes

            float readout = float.Parse(strEul[0]);
            readout = map(readout, 0f, 1023f, -4f, 4f);
            verticalMovement = readout * Time.deltaTime * speed;
            rotateUp = map(readout, -4f, 4f, -45f, 45f)  * Time.deltaTime * speed * -1;

            readout = float.Parse(strEul[2]);
            readout = map(readout, 0f, 1023f, -4f, 4f);
            horizontalMovement = readout * -1 * Time.deltaTime * speed;
            rotateRight = map(readout, -4f, 4f, -45f, 45f) * Time.deltaTime * speed;


            Debug.Log("Horizontal: " + horizontalMovement);
            Debug.Log("Vertical: " + verticalMovement);
            aircraft.transform.Translate(new Vector3(horizontalMovement, verticalMovement, forwardSpeed * Time.deltaTime));
            aircraft.transform.Rotate(rotateUp, 0 , rotateRight);

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