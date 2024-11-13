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
                // ����Ϊ��������ֵ��public����
                 sensorA0 = int.Parse(sensorValues[0]);
                sensorA1 = int.Parse(sensorValues[1]);
            }
        
        
        
        
        // ��������speedXֵӳ�䵽�ٶ�
        float speedX = Mathf.Lerp(0, maxSpeed, sensorA0 / 1024f); // X���ٶ�
        float speedZ = Mathf.Lerp(0, maxSpeed, sensorA1 / 1024f); // Z���ٶ�
        Debug.Log(speedX + speedZ);
        // �����ƶ�����
        Vector3 movement = new Vector3(speedX * Time.deltaTime, 0, speedZ * Time.deltaTime);

        // ��������λ��
        controlledObject.transform.position += movement;
    }
}
