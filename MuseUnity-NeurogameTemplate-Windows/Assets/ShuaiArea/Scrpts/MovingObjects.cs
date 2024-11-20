using System.Collections.Generic;
using UnityEngine;

public class MovingObjects : MonoBehaviour
{ // 预制体
    public GameObject prefab;
    public GameObject waveFloor;
    public Light spotlight;

    [Range(1, 179)] // 聚光灯的角度范围，通常是1到179度
    public float spotAngle = 60f;

    [Range(0, 100)] // 聚光灯的强度范围，可以根据需要调整
    public float intensity = 40f;

    // 矩阵的行和列数量
    public int rows = 5;
    public int columns = 5;


    public float spacing = 1.5f; // 小球之间的间距
    public float floatAmplitude = 0.5f; // 浮动的幅度
    public float floatFrequency = 1.0f; // 浮动的频率

    private List<GameObject> prefabs = new List<GameObject>(); // 存储所有生成的预制体
    private Vector3 center; // 中心位置

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
        // 更新预制体位置，使其浮动
        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject instance = prefabs[i];
            // 获取初始位置
            Vector3 startPos = instance.transform.position;
            // 根据正弦函数计算浮动的偏移
            float offset = Mathf.Sin(Time.time * floatFrequency + i * 0.5f) * floatAmplitude;
            // Debug.Log(offset);
            // 更新预制体位置
            instance.transform.position = new Vector3(startPos.x, center.y + offset, startPos.z);
            waveFloor.transform.position = new Vector3(waveFloor.transform.position.x, waveFloor.transform.position.y + offset / 100, waveFloor.transform.position.z);
        }
    }
    private void generateArray()
    {
        // 确定中心位置
        center = transform.position;

        // 创建Prefab矩阵
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // 计算每个预制体的位置，基于中心位置计算
                float xOffset = (i - (rows - 1) / 2.0f) * spacing;
                float zOffset = (j - (columns - 1) / 2.0f) * spacing;
                Vector3 position = new Vector3(center.x + xOffset, center.y, center.z + zOffset);

                // 实例化预制体
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);
                instance.transform.SetParent(transform); // 设置为当前对象的子对象
                prefabs.Add(instance); // 添加到列表中
            }
        }
    }
}
