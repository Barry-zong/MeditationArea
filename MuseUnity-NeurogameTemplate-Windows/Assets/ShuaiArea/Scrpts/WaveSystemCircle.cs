using UnityEngine;
using System.Collections.Generic;
using Interaxon.Libmuse;

public class WaveSystemCircle : MonoBehaviour
{
    [Header("Wave Generation Settings")]
    public GameObject waveCenter;
    public GameObject prefab;
    public GameObject PlayerBug; // 新增Player引用
    public float outerRadius = 5f;
    public float innerRadius = 2f;
    public float ringSpacing = 1f;
    public float elementScale = 1f;

    [Header("Distance Scale Settings")] // 新增距离缩放相关设置
    public float minDistance = 1f;
    public float maxDistance = 10f;
    public float minScaleBonus = 0f;
    public float maxScaleBonus = 0.2f;

    [Header("Performance Settings")]
    [Range(3, 20)]
    public int maxRings = 10;
    [Range(4, 30)]
    public int elementsPerRing = 20;

    [Header("Wave Movement Settings")]
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 0.5f;

    [Header("Cleanup Settings")]
    public float destroyDelay = 0.5f;

    private List<GameObject> prefabs = new List<GameObject>();
    private Vector3 center;
    public float currentMindFocus = 0;
    public float targetMindFocus = 0;
    public bool isDebug = true;
    public float mindValue = 0.5f;
   
    private int totalElements = 0;
    private bool isInitialized = false;
    [Header("wavingBalls")]
    public Material wavingBalls;

    public FlowRoomSwitch flowRoomSwitch;

    void Start()
    {
        wavingBalls.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, -5f));
        if (!isInitialized)
        {
            GenerateCircularWaveArray();
            isInitialized = true;
        }
    }

    void Update()
    {
        if (!flowRoomSwitch.flowStartBool) return;
        if (PlayerBug == null) return;

        wavingBalls.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, -5 + (currentMindFocus / 20) * 7));
        floatAmplitude = 2.2f - currentMindFocus/10;

        prefabs.RemoveAll(item => item == null);

        UpdateMindFocus();
        currentMindFocus = Mathf.Lerp(currentMindFocus, targetMindFocus, Time.deltaTime * 2.0f);

        if (prefabs.Count == 0 && isInitialized)
        {
            GenerateCircularWaveArray();
        }

        UpdateWaveMovement();
        UpdateBallsScale(); // 新增更新小球尺寸的调用
    }

    // 新增：更新小球尺寸的方法
    private void UpdateBallsScale()
    {
        foreach (var ball in prefabs)
        {
            if (ball != null)
            {
                float distance = Vector3.Distance(ball.transform.position, PlayerBug.transform.position);
                Debug.Log(distance);
                float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
                float newBallsScale = Mathf.Lerp(maxScaleBonus, minScaleBonus, normalizedDistance);
                
                // 更新小球尺寸：基础尺寸 + 距离带来的额外尺寸
                ball.transform.localScale = Vector3.one * (elementScale + newBallsScale);
            }
        }
    }

    private void CleanupExistingPrefabs()
    {
        foreach (var obj in prefabs)
        {
            if (obj != null)
            {
                Destroy(obj, destroyDelay);
            }
        }
        prefabs.Clear();
        totalElements = 0;
    }

    private void GenerateCircularWaveArray()
    {
        if (prefab == null) return;

        CleanupExistingPrefabs();

        center = waveCenter != null ? waveCenter.transform.position : transform.position;

        int calculatedRings = Mathf.FloorToInt((outerRadius - innerRadius) / ringSpacing);
        int numberOfRings = Mathf.Min(calculatedRings, maxRings);
        float adjustedRingSpacing = (outerRadius - innerRadius) / numberOfRings;

        for (int ring = 0; ring < numberOfRings; ring++)
        {
            float currentRadius = innerRadius + (ring * adjustedRingSpacing);
            float circumference = 2f * Mathf.PI * currentRadius;
            int calculatedElements = Mathf.FloorToInt(circumference / (elementScale * 1.2f));
            int numberOfElements = Mathf.Min(calculatedElements, elementsPerRing);

            for (int i = 0; i < numberOfElements; i++)
            {
                float angle = ((float)i / numberOfElements) * 2f * Mathf.PI;
                float x = center.x + currentRadius * Mathf.Cos(angle);
                float z = center.z + currentRadius * Mathf.Sin(angle);
                Vector3 position = new Vector3(x, center.y, z);

                GameObject instance = Instantiate(prefab, position, Quaternion.identity, transform);
                if (instance != null)
                {
                    Vector3 directionToCenter = center - position;
                    directionToCenter.y = 0;
                    instance.transform.rotation = Quaternion.LookRotation(directionToCenter);
                    instance.transform.localScale = Vector3.one * elementScale;
                    prefabs.Add(instance);
                    totalElements++;
                }
            }
        }
    }

    private void UpdateWaveMovement()
    {
        for (int i = prefabs.Count - 1; i >= 0; i--)
        {
            if (i >= prefabs.Count) continue;

            GameObject instance = prefabs[i];
            if (instance == null)
            {
                prefabs.RemoveAt(i);
                continue;
            }

            Vector3 startPos = instance.transform.position;
            float distanceFromCenter = Vector3.Distance(
                new Vector3(startPos.x, 0, startPos.z),
                new Vector3(center.x, 0, center.z)
            );

            float phaseOffset = distanceFromCenter * 0.5f;
            float offset = Mathf.Sin(Time.time * (floatFrequency * (isDebug ? 1 : currentMindFocus)) + phaseOffset)
                          * (floatAmplitude * (isDebug ? 1 : currentMindFocus) / 2);

            instance.transform.position = new Vector3(startPos.x, center.y + offset, startPos.z);
        }
    }

    private void UpdateMindFocus()
    {
        if (isDebug)
        {
            targetMindFocus = Mathf.Clamp(mindValue * 10, 0, 22);
        }
        else
        {
            if (InteraxonInterfacer.Instance != null)
            {
                targetMindFocus = Mathf.Clamp(InteraxonInterfacer.Instance.flow * 10, 0, 22);
            }
            else
            {
                targetMindFocus = Mathf.Clamp(mindValue * 10, 0, 22);
            }
        }
    }

    void OnValidate()
    {
        if (outerRadius < innerRadius) outerRadius = innerRadius;
        if (innerRadius < 0) innerRadius = 0;
        if (ringSpacing < 0.1f) ringSpacing = 0.1f;
        if (elementScale < 0.1f) elementScale = 0.1f;
        if (floatAmplitude < 0) floatAmplitude = 0;
        if (floatFrequency < 0) floatFrequency = 0;
        if (destroyDelay < 0.1f) destroyDelay = 0.1f;
        
        // 新增距离相关参数的验证
        if (minDistance < 0) minDistance = 0;
        if (maxDistance < minDistance) maxDistance = minDistance + 1;
        if (minScaleBonus < 0) minScaleBonus = 0;
        if (maxScaleBonus < minScaleBonus) maxScaleBonus = minScaleBonus;
    }

    void OnDisable()
    {
        foreach (var obj in prefabs)
        {
            if (obj != null)
            {
                Destroy(obj, destroyDelay);
            }
        }
        prefabs.Clear();
    }
}