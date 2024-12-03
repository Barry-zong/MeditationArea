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
    public GameObject BoxDoor;
    public float doorFloatRange = 2f;
    public float doorFloatSpeed = 2f;
    public float doorMinHeight = 0f;
    public float doorMaxHeight = 5f;

    [Header("Moon Control")]
    public GameObject Moon;
    public float moonMinScale = 0.5f;
    public float moonMaxScale = 2f;
    public float moonFloatRange = 3f;
    public float moonFloatSpeed = 1f;
    public float moonScaleSpeed = 1.5f;

    [Header("Flow Light Control")]
    public Light flowLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 3f;
    public float lightChangeSpeed = 1f;

    [Header("Focus Spotlight Control")]
    public Light playerLight;                  // 聚光灯组件
    public float spotMinIntensity = 0.2f;     // 最小亮度（focus为0时）
    public float spotMaxIntensity = 5f;       // 最大亮度
    public float spotChangeSpeed = 1.5f;      // 亮度变化速度

    private List<GameObject> prefabs = new List<GameObject>();
    private Vector3 center;
    private float targetMindFocus = 0;
    private float currentMindFocus = 0;
    private Vector3 doorStartPosition;
    private float currentDoorHeight;
    private Vector3 moonStartPosition;
    private float currentMoonHeight;
    private float currentMoonScale = 1f;
    private float targetFlow = 0;
    private float currentFlow = 0;

    public RoomChangeController roomChangeCC;

    void Start()
    {
        generateArray();

        if (BoxDoor != null)
        {
            doorStartPosition = BoxDoor.transform.position;
            currentDoorHeight = doorStartPosition.y;
        }

        if (Moon != null )
        {
            Debug.Log(roomChangeCC.triggerActivated);
            moonStartPosition = Moon.transform.position;
            currentMoonHeight = moonStartPosition.y;
           
              
                currentMoonScale = Moon.transform.localScale.x;
            
               
        }

        if (flowLight != null)
        {
            flowLight.intensity = minIntensity;
        }

        if (playerLight != null)
        {
            playerLight.intensity = spotMinIntensity;
        }
    }

    void Update()
    {
        UpdateMindFocus();
        UpdateFlowState();

        currentMindFocus = Mathf.Lerp(currentMindFocus, targetMindFocus, Time.deltaTime * 2.0f);
        currentFlow = Mathf.Lerp(currentFlow, targetFlow, Time.deltaTime * 2.0f);

        waveElement();
        UpdateDoorPosition();
        UpdateMoonState();
        UpdateLightIntensity();
        UpdateSpotlightIntensity();
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

    private void UpdateFlowState()
    {
        if (InteraxonInterfacer.Instance != null &&
            InteraxonInterfacer.Instance.currentConnectionState == ConnectionState.CONNECTED &&
            InteraxonInterfacer.Instance.Artifacts.headbandOn)
        {
            targetFlow = Mathf.Clamp(InteraxonInterfacer.Instance.calm * 10, 0, 10);
        }
        else
        {
            targetFlow = 0;
        }
    }

    private void UpdateDoorPosition()
    {
        if (BoxDoor == null) return;

        float targetHeight = doorStartPosition.y + (currentMindFocus / 10f) * doorFloatRange;
        targetHeight = Mathf.Clamp(targetHeight, doorMinHeight, doorMaxHeight);
        currentDoorHeight = Mathf.Lerp(currentDoorHeight, targetHeight, Time.deltaTime * doorFloatSpeed);

        BoxDoor.transform.position = new Vector3(
            doorStartPosition.x,
            currentDoorHeight,
            doorStartPosition.z
        );
    }

    private void UpdateMoonState()
    {
        if (Moon == null) return;

        // 更新月亮高度
        float targetHeight = moonStartPosition.y + (currentFlow / 10f) * moonFloatRange;
        currentMoonHeight = Mathf.Lerp(currentMoonHeight, targetHeight, Time.deltaTime * moonFloatSpeed);

        // 更新月亮位置
        Moon.transform.position = new Vector3(
            moonStartPosition.x,
            currentMoonHeight,
            moonStartPosition.z
        );
        if (roomChangeCC.triggerActivated)
        {
            // 更新月亮大小
            float targetScale = Mathf.Lerp(moonMinScale, moonMaxScale, currentFlow / 10f);
            currentMoonScale = Mathf.Lerp(currentMoonScale, targetScale, Time.deltaTime * moonScaleSpeed);
            Moon.transform.localScale = Vector3.one * currentMoonScale;
        }
       
    }

    private void UpdateLightIntensity()
    {
        if (flowLight == null) return;
        if (roomChangeCC.triggerActivated)
        {
            float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, currentFlow / 10f);
            flowLight.intensity = Mathf.Lerp(flowLight.intensity, targetIntensity, Time.deltaTime * lightChangeSpeed);
        }
    }

    private void UpdateSpotlightIntensity()
    {
        if (playerLight == null) return;

        float targetIntensity = Mathf.Lerp(spotMinIntensity, spotMaxIntensity, currentMindFocus / 10f);
        playerLight.intensity = Mathf.Lerp(playerLight.intensity, targetIntensity, Time.deltaTime * spotChangeSpeed);
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

    void OnValidate()
    {
        if (doorMinHeight > doorMaxHeight)
        {
            doorMinHeight = doorMaxHeight;
        }
        if (doorFloatRange < 0)
        {
            doorFloatRange = 0;
        }
        if (doorFloatSpeed < 0)
        {
            doorFloatSpeed = 0;
        }

        if (moonMinScale > moonMaxScale)
        {
            moonMinScale = moonMaxScale;
        }
        if (minIntensity > maxIntensity)
        {
            minIntensity = maxIntensity;
        }
        if (moonFloatRange < 0)
        {
            moonFloatRange = 0;
        }
        if (moonFloatSpeed < 0)
        {
            moonFloatSpeed = 0;
        }
        if (lightChangeSpeed < 0)
        {
            lightChangeSpeed = 0;
        }

        if (spotMinIntensity > spotMaxIntensity)
        {
            spotMinIntensity = spotMaxIntensity;
        }
        if (spotMinIntensity < 0)
        {
            spotMinIntensity = 0;
        }
        if (spotChangeSpeed < 0)
        {
            spotChangeSpeed = 0;
        }
    }
}