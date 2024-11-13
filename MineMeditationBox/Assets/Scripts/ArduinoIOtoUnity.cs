using System.Collections;
using UnityEngine;

public class ArduinoIOtoUnity : MonoBehaviour
{
    public SerialController serialController;
    public float arduinoPortNumberOne = 0;
    public float arduinoPortNumberTwo = 0;
    public string ArduinoMessage = string.Empty;


    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    void Update()
    {
        // �Ӵ��ڽ�������
        ArduinoMessage = serialController.ReadSerialMessage();
        string message = serialController.ReadSerialMessage();
        

        if (message == null)
            return;
        ParseData(message);

        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
        {
            Debug.Log("Arduino ������");
        }
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
        {
            Debug.Log("Arduino �ѶϿ�");
        }
        else
        {
          //  Debug.Log("Received: " + message); // ��ʾ���յ�������
        }
    }
    void ParseData(string data)
    {
        // �����ݰ����ŷָ�
        string[] sensorValues = data.Split(',');
        if (sensorValues.Length >= 2)
        {
            // ����Ϊ��������ֵ��public����
            arduinoPortNumberOne = int.Parse(sensorValues[0]);
            arduinoPortNumberTwo = int.Parse(sensorValues[1]);
        }
    }
}
