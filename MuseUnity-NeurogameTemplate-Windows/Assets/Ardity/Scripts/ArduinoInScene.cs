using UnityEngine;

public class ArduinoInScene : MonoBehaviour
{
    public SerialController serialController;
    public float arduinoPortNumberOne = 0;
    public float arduinoPortNumberTwo = 0;
    public float arduinoPortNumberThree = 0;
    public string ArduinoMessage = string.Empty;
   private string message;


    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }
    void OnMessageArrived(string msg)
    {
        message = msg;
        Debug.Log("Message arrived: " + msg);
    }


    void Update()
    {
        // 从串口接收数据
        // ArduinoMessage = serialController.ReadSerialMessage();

        ArduinoMessage = message;
         if (message == null)
            return;
        ParseData(message);

        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
        {
            Debug.Log("Arduino 已连接");
        }
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
        {
            Debug.Log("Arduino 已断开");
        }
        else
        {
            //  Debug.Log("Received: " + message); // 显示接收到的数据
        }
    }
    void ParseData(string data)
    {
        // 将数据按逗号分割
        string[] sensorValues = data.Split(',');
        if (sensorValues.Length >= 3)
        {
            // 解析为整数并赋值到public变量
            arduinoPortNumberOne = int.Parse(sensorValues[0]);
            arduinoPortNumberTwo = int.Parse(sensorValues[1]);
            arduinoPortNumberThree = int.Parse(sensorValues[2]);
        }
    }
}
