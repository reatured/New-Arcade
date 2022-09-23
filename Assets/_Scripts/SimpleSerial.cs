using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SimpleSerial : MonoBehaviour
{

    public String portName = "COM15";  // use the port name for your Arduino, such as /dev/tty.usbmodem1411 for Mac or COM3 for PC
    public GameObject leftPaddle;

    private SerialPort serialPort = null; 
    private int baudRate = 115200;  // match your rate from your serial in Arduino
    private int readTimeOut = 100;

    private string serialInput;

    bool programActive = true;
    Thread thread;

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
                if (int.Parse(strEul[1]) == 0) // if button pressed
                {
                   

                }
                else
                {
                   

                }
                float readout = float.Parse(strEul[0]);
                readout = Mathf.Clamp(readout, 200f, 824f);
                readout = map(readout, 200f, 824f, -4f, 4f);
                //flip y
                readout *= -1f;

                Vector3 cPos = leftPaddle.transform.position;
                cPos = new Vector3(cPos.x, readout, cPos.z);
                leftPaddle.transform.position = cPos;
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