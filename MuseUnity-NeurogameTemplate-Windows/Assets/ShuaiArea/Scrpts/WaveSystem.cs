using UnityEngine;
using System.Collections.Generic;
using Interaxon.Libmuse;

public class WaveSystem : MonoBehaviour
{
    [Header("Wave Generation Settings")]
    public GameObject waveCenter;      // ЙъіЙЦРРДОпМе
    public GameObject prefab;          // ІЁАЛФЄЛШФ¤ЦЖМе
    public int rows = 5;              // РРКэ
    public int columns = 5;           // БРКэ
    public float spacing = 1.5f;      // јдѕа

    [Header("Wave Movement Settings")]
    public float floatAmplitude = 0.2f;    // ёЎ¶Ї·щ¶И
    public float floatFrequency = 0.5f;    // ёЎ¶ЇЖµВК

    private List<GameObject> prefabs = new List<GameObject>();
    private Vector3 center;
    private float currentMindFocus = 0;     // µ±З°ЧЁЧў¶ИЦµ
    private float targetMindFocus = 0;      // Дї±кЧЁЧў¶ИЦµ

    public bool isDebug = false;
    public float mindValue = 0;

    void Start()
    {
        GenerateWaveArray();
    }

    void Update()
    {
        UpdateMindFocus();
        currentMindFocus = Mathf.Lerp(currentMindFocus, targetMindFocus, Time.deltaTime * 2.0f);
        UpdateWaveMovement();
    }

    // ЙъіЙІЁАЛХуБР
    private void GenerateWaveArray()
    {
        center = waveCenter != null ? waveCenter.transform.position : transform.position;

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

    // ёьРВІЁАЛФЛ¶Ї
    private void UpdateWaveMovement()
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

    // Ц±ЅУґУ InteraxonInterfacer »сИЎЧЁЧў¶ИЦµ
    private void UpdateMindFocus()
    {
        // switch input
        if (isDebug)
        {
            targetMindFocus = Mathf.Clamp(mindValue * 10, 0, 10);
        }
        else
        {
            targetMindFocus = Mathf.Clamp(InteraxonInterfacer.Instance.focus * 10, 0, 10);
        }

    }

    void OnValidate()
    {
        if (spacing < 0) spacing = 0;
        if (floatAmplitude < 0) floatAmplitude = 0;
        if (floatFrequency < 0) floatFrequency = 0;
    }
}