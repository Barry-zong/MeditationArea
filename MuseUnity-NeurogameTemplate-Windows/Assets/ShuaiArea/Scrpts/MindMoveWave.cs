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
    public GameObject BoxDoor;            // ��Ҫ���Ƶ���
    public float doorFloatRange = 2f;     // �Ÿ��������Χ
    public float doorFloatSpeed = 2f;     // �Ÿ������ٶ�
    public float doorMinHeight = 0f;      // �Ÿ�������С�߶�����
    public float doorMaxHeight = 5f;      // �Ÿ��������߶�����

    private List<GameObject> prefabs = new List<GameObject>();
    private Vector3 center;
    private float targetMindFocus = 0;
    private float currentMindFocus = 0;
    private Vector3 doorStartPosition;    // �ŵĳ�ʼλ��
    private float currentDoorHeight;      // �ŵ�ǰ�ĸ߶�

    void Start()
    {
        generateArray();

        // �����ŵĳ�ʼλ��
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

        // ����Ŀ��߶ȣ���ʼ�߶� + (רע�� * ������Χ)
        float targetHeight = doorStartPosition.y + (currentMindFocus / 10f) * doorFloatRange;

        // ���Ƹ߶����趨��Χ��
        targetHeight = Mathf.Clamp(targetHeight, doorMinHeight, doorMaxHeight);

        // ƽ�����ɵ�Ŀ��߶�
        currentDoorHeight = Mathf.Lerp(currentDoorHeight, targetHeight, Time.deltaTime * doorFloatSpeed);

        // �����ŵ�λ��
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

    // �ڱ༭������֤����
    void OnValidate()
    {
        // ȷ����С�߶Ȳ��������߶�
        if (doorMinHeight > doorMaxHeight)
        {
            doorMinHeight = doorMaxHeight;
        }

        // ȷ��������ΧΪ��ֵ
        if (doorFloatRange < 0)
        {
            doorFloatRange = 0;
        }

        // ȷ�������ٶ�Ϊ��ֵ
        if (doorFloatSpeed < 0)
        {
            doorFloatSpeed = 0;
        }
    }
}