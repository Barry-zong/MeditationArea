using System.Collections.Generic;
using UnityEngine;

public class MovingObjects : MonoBehaviour
{ // Ԥ����
    public GameObject prefab;
    public GameObject waveFloor;
    public Light spotlight;

    [Range(1, 179)] // �۹�ƵĽǶȷ�Χ��ͨ����1��179��
    public float spotAngle = 60f;

    [Range(0, 100)] // �۹�Ƶ�ǿ�ȷ�Χ�����Ը�����Ҫ����
    public float intensity = 40f;

    // ������к�������
    public int rows = 5;
    public int columns = 5;


    public float spacing = 1.5f; // С��֮��ļ��
    public float floatAmplitude = 0.5f; // �����ķ���
    public float floatFrequency = 1.0f; // ������Ƶ��

    private List<GameObject> prefabs = new List<GameObject>(); // �洢�������ɵ�Ԥ����
    private Vector3 center; // ����λ��

    void Start()
    {

        generateArray();
    }

    void Update()
    {
        waveElement();
        updateSpotLight();

    }

    private void updateSpotLight()
    {
        if (spotlight != null)
        {
            spotlight.spotAngle = spotAngle;
            spotlight.intensity = intensity;
        }
    }

    private void waveElement()
    {
        // ����Ԥ����λ�ã�ʹ�両��
        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject instance = prefabs[i];
            // ��ȡ��ʼλ��
            Vector3 startPos = instance.transform.position;
            // �������Һ������㸡����ƫ��
            float offset = Mathf.Sin(Time.time * floatFrequency + i * 0.5f) * floatAmplitude;
            // Debug.Log(offset);
            // ����Ԥ����λ��
            instance.transform.position = new Vector3(startPos.x, center.y + offset, startPos.z);
            waveFloor.transform.position = new Vector3(waveFloor.transform.position.x, waveFloor.transform.position.y + offset / 100, waveFloor.transform.position.z);
        }
    }
    private void generateArray()
    {
        // ȷ������λ��
        center = transform.position;

        // ����Prefab����
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // ����ÿ��Ԥ�����λ�ã���������λ�ü���
                float xOffset = (i - (rows - 1) / 2.0f) * spacing;
                float zOffset = (j - (columns - 1) / 2.0f) * spacing;
                Vector3 position = new Vector3(center.x + xOffset, center.y, center.z + zOffset);

                // ʵ����Ԥ����
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);
                instance.transform.SetParent(transform); // ����Ϊ��ǰ������Ӷ���
                prefabs.Add(instance); // ��ӵ��б���
            }
        }
    }
}
