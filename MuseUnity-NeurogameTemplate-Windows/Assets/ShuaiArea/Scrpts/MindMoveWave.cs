using System.Collections.Generic;
using UnityEngine;
using Interaxon.Libmuse;

public class MindMoveWave : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject prefab;
    public int rows = 5;
    public int columns = 5;
    public float spacing = 1.5f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 0.5f;

    [Header("Door Control")]
    public GameObject BoxDoor;            // 需要控制的门
    public float doorFloatRange = 2f;     // 门浮动的最大范围
    public float doorFloatSpeed = 2f;     // 门浮动的速度
    public float doorMinHeight = 0f;      // 门浮动的最小高度限制
    public float doorMaxHeight = 5f;      // 门浮动的最大高度限制

    private List<GameObject> prefabs = new List<GameObject>();
    private Vector3 center;
    private float targetMindFocus = 0;
    private float currentMindFocus = 0;
    private Vector3 doorStartPosition;    // 门的初始位置
    private float currentDoorHeight;      // 门当前的高度

    void Start()
    {
        generateArray();

        // 保存门的初始位置
        if (BoxDoor != null)
        {
            doorStartPosition = BoxDoor.transform.position;
            currentDoorHeight = doorStartPosition.y;
        }
    }

    void Update()
    {
        UpdateMindFocus();
        currentMindFocus = Mathf.Lerp(currentMindFocus, targetMindFocus, Time.deltaTime * 2.0f);
        waveElement();
        UpdateDoorPosition();
    }

    private void UpdateMindFocus()
    {
        if (InteraxonInterfacer.Instance != null &&
            InteraxonInterfacer.Instance.currentConnectionState == ConnectionState.CONNECTED &&
            InteraxonInterfacer.Instance.Artifacts.headbandOn)
        {
            targetMindFocus = Mathf.Clamp(InteraxonInterfacer.Instance.focus * 10, 0, 10);
        }
        else
        {
            targetMindFocus = 0;
        }
    }

    private void UpdateDoorPosition()
    {
        if (BoxDoor == null) return;

        // 计算目标高度：初始高度 + (专注度 * 浮动范围)
        float targetHeight = doorStartPosition.y + (currentMindFocus / 10f) * doorFloatRange;

        // 限制高度在设定范围内
        targetHeight = Mathf.Clamp(targetHeight, doorMinHeight, doorMaxHeight);

        // 平滑过渡到目标高度
        currentDoorHeight = Mathf.Lerp(currentDoorHeight, targetHeight, Time.deltaTime * doorFloatSpeed);

        // 更新门的位置
        BoxDoor.transform.position = new Vector3(
            doorStartPosition.x,
            currentDoorHeight,
            doorStartPosition.z
        );
    }

    private void waveElement()
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject instance = prefabs[i];
            Vector3 startPos = instance.transform.position;
            float offset = Mathf.Sin(Time.time * (floatFrequency * currentMindFocus) + i * 0.5f)
                          * (floatAmplitude * currentMindFocus / 2);
            instance.transform.position = new Vector3(startPos.x, center.y + offset, startPos.z);
        }
    }

    private void generateArray()
    {
        center = transform.position;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                float xOffset = (i - (rows - 1) / 2.0f) * spacing;
                float zOffset = (j - (columns - 1) / 2.0f) * spacing;
                Vector3 position = new Vector3(center.x + xOffset, center.y, center.z + zOffset);
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);
                instance.transform.SetParent(transform);
                prefabs.Add(instance);
            }
        }
    }

    // 在编辑器中验证参数
    void OnValidate()
    {
        // 确保最小高度不大于最大高度
        if (doorMinHeight > doorMaxHeight)
        {
            doorMinHeight = doorMaxHeight;
        }

        // 确保浮动范围为正值
        if (doorFloatRange < 0)
        {
            doorFloatRange = 0;
        }

        // 确保浮动速度为正值
        if (doorFloatSpeed < 0)
        {
            doorFloatSpeed = 0;
        }
    }
}