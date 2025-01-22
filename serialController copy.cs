using System;
using System.IO.Ports;
using UnityEngine;

public class SerialController : MonoBehaviour
{
    public string portName = "/dev/cu.usbserial-0001"; // Skift til din port (f.eks. "COM3")
    public int baudRate = 9600; // Match baudraten på ESP32

    private SerialPort serialPort;
    public CarController car; // Reference til CarController scriptet

    void Start()
    {
        // Initialiserer og åbner den serielle port
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }

    void Update()
    {
        if (serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            try
            {
                // Læs serielle data
                string data = serialPort.ReadLine();
                ProcessData(data);
            }
            catch (Exception e)
            {
                Debug.LogError("Fejl ved læsning af seriel data: " + e.Message);
            }
        }
    }

    void ProcessData(string data)
    {
        // Forventet format: "potValue,forward,backward"
        string[] values = data.Split(',');

        if (values.Length == 3)
        {
            // Læs og normaliser potentiometerets værdi
            float potValue = float.Parse(values[0]);
            float steeringInput = (potValue - 2048) / 2048; // Normaliseret til -1 til 1

            // Bestem fremad og bagud bevægelse
            float moveInput = (values[1] == "1" ? 1 : (values[2] == "1" ? -1 : 0));

            // Send data til bilens kontrolscript
            car.SetInputs(moveInput, steeringInput);
        }
    }

    void OnDestroy()
    {
        // Luk seriel port ved ødelæggelse
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}