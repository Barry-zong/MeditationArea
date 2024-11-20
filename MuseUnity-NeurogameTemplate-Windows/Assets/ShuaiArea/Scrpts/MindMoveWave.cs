using System.Collections.Generic;
using UnityEngine;
using Interaxon.Libmuse;

public class MindMoveWave : MonoBehaviour
{
    public GameObject prefab;

    // 矩阵的行和列数量
    public int rows = 5;
    public int columns = 5;

    public float spacing = 1.5f; // 小球之间的间距
    public float floatAmplitude = 0.2f; // 浮动的基础幅度
    public float floatFrequency = 0.5f; // 浮动的基础频率

    private List<GameObject> prefabs = new List<GameObject>(); // 存储所有生成的预制体
    private Vector3 center; // 中心位置

    private float targetMindFocus = 0; // 从设备获取的目标值
    private float currentMindFocus = 0; // 插值平滑过渡的当前值

    void Start()
    {
        generateArray();
    }

    void Update()
    {
        // 更新 MindFocus 目标值
        UpdateMindFocus();

        // 平滑过渡到目标值
        currentMindFocus = Mathf.Lerp(currentMindFocus, targetMindFocus, Time.deltaTime * 2.0f);

        // 更新浮动逻辑
        waveElement();
    }

    private void UpdateMindFocus()
    {
        // 模拟从 Interaxon 获取 focus 值
        if (InteraxonInterfacer.Instance != null && InteraxonInterfacer.Instance.currentConnectionState == ConnectionState.CONNECTED &&
            InteraxonInterfacer.Instance.Artifacts.headbandOn)
        {
            targetMindFocus = Mathf.Clamp(InteraxonInterfacer.Instance.focus * 10, 0, 10); // 限制范围到 0-10
        }
        else
        {
            targetMindFocus = 0; // 未连接时的默认值
        }

        Debug.Log($"Target MindFocus: {targetMindFocus}, Current MindFocus: {currentMindFocus}");
    }

    private void waveElement()
    {
        // 更新预制体位置，使其浮动
        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject instance = prefabs[i];
            // 获取初始位置
            Vector3 startPos = instance.transform.position;

            // 根据正弦函数计算浮动的偏移，加入 MindFocus 的影响
            float offset = Mathf.Sin(Time.time * (floatFrequency * currentMindFocus) + i * 0.5f) * (floatAmplitude * currentMindFocus / 2);

            // 更新预制体位置
            instance.transform.position = new Vector3(startPos.x, center.y + offset, startPos.z);
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
