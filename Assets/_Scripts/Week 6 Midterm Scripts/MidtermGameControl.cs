using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

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
    // Start is called before the first frame update
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
        //arduino script above

        //my script:
        for(int i = 0; i < 100; i++)
        {
            float x = UnityEngine.Random.Range(xMin, xMax);
            float z = UnityEngine.Random.Range(zMin, zMax);
            Instantiate(tree, new Vector3(x, -0.226f, z), Quaternion.identity);
        }
        

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

    // Update is called once per frame
    void Update()
    {
        if (serialInput != null)
        {
            string[] strEul = serialInput.Split(';');  // parses using semicolon ; into a string array called strEul. I originally was sending Euler angles for gyroscopes
            float readout = float.Parse(strEul[0]);
            frogReading = readout;

            readout = float.Parse(strEul[2]);
            owlReading = readout; 
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
}
