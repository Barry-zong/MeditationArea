using System.Collections.Generic;
using UnityEngine;
using Interaxon.Libmuse;

public class MindMoveWave : MonoBehaviour
{
    public GameObject prefab;

    // ������к�������
    public int rows = 5;
    public int columns = 5;

    public float spacing = 1.5f; // С��֮��ļ��
    public float floatAmplitude = 0.2f; // �����Ļ�������
    public float floatFrequency = 0.5f; // �����Ļ���Ƶ��

    private List<GameObject> prefabs = new List<GameObject>(); // �洢�������ɵ�Ԥ����
    private Vector3 center; // ����λ��

    private float targetMindFocus = 0; // ���豸��ȡ��Ŀ��ֵ
    private float currentMindFocus = 0; // ��ֵƽ�����ɵĵ�ǰֵ

    void Start()
    {
        generateArray();
    }

    void Update()
    {
        // ���� MindFocus Ŀ��ֵ
        UpdateMindFocus();

        // ƽ�����ɵ�Ŀ��ֵ
        currentMindFocus = Mathf.Lerp(currentMindFocus, targetMindFocus, Time.deltaTime * 2.0f);

        // ���¸����߼�
        waveElement();
    }

    private void UpdateMindFocus()
    {
        // ģ��� Interaxon ��ȡ focus ֵ
        if (InteraxonInterfacer.Instance != null && InteraxonInterfacer.Instance.currentConnectionState == ConnectionState.CONNECTED &&
            InteraxonInterfacer.Instance.Artifacts.headbandOn)
        {
            targetMindFocus = Mathf.Clamp(InteraxonInterfacer.Instance.focus * 10, 0, 10); // ���Ʒ�Χ�� 0-10
        }
        else
        {
            targetMindFocus = 0; // δ����ʱ��Ĭ��ֵ
        }

        Debug.Log($"Target MindFocus: {targetMindFocus}, Current MindFocus: {currentMindFocus}");
    }

    private void waveElement()
    {
        // ����Ԥ����λ�ã�ʹ�両��
        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject instance = prefabs[i];
            // ��ȡ��ʼλ��
            Vector3 startPos = instance.transform.position;

            // �������Һ������㸡����ƫ�ƣ����� MindFocus ��Ӱ��
            float offset = Mathf.Sin(Time.time * (floatFrequency * currentMindFocus) + i * 0.5f) * (floatAmplitude * currentMindFocus / 2);

            // ����Ԥ����λ��
            instance.transform.position = new Vector3(startPos.x, center.y + offset, startPos.z);
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
