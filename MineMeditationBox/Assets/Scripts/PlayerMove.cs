using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameObject controlledObject;
    public float maxSpeed = 10f;
    public ArduinoIOtoUnity dataReceiver;
    private string messageA;
    public ArduinoIOtoUnity arduinotounity;
    private float  sensorA1;

    private float sensorA0;
    // Start is called before the first frame update
    void Start()
    {
        messageA = arduinotounity.ArduinoMessage;
    }

    // Update is called once per frame
    void Update()
    {


            messageA = arduinotounity.ArduinoMessage;
            string[] sensorValues = messageA.Split(',');
            Debug.Log(messageA);
            if (sensorValues.Length >= 2)
            {
                // 解析为整数并赋值到public变量
                 sensorA0 = int.Parse(sensorValues[0]);
                sensorA1 = int.Parse(sensorValues[1]);
            }
        
        
        
        
        // 将传感器speedX值映射到速度
        float speedX = Mathf.Lerp(0, maxSpeed, sensorA0 / 1024f); // X轴速度
        float speedZ = Mathf.Lerp(0, maxSpeed, sensorA1 / 1024f); // Z轴速度
        Debug.Log(speedX + speedZ);
        // 计算移动增量
        Vector3 movement = new Vector3(speedX * Time.deltaTime, 0, speedZ * Time.deltaTime);

        // 更新物体位置
        controlledObject.transform.position += movement;
    }
}
