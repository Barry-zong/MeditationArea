using UnityEngine;
using System.Collections.Generic;

public class ScaleTransition : MonoBehaviour
{
    [Tooltip("需要进行缩放过渡的游戏对象列表")]
    public List<GameObject> transitionWhenStart = new List<GameObject>();

    [Tooltip("初始缩放值")]
    public float InitialScale = 0f;

    [Tooltip("完成缩放所需时间(秒)")]
    public float scaleTime = 1.0f;

    [Tooltip("是否逐个缩放")]
    public bool scaleOneByOne = false;

    [Tooltip("逐个缩放的时间间隔")]
    public float OneByOneTime = 0.5f;

    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, float> objectStartTimes = new Dictionary<GameObject, float>();
    private bool isScaling = false;
    private float triggerTime = 0f;
    public bool First = true;

    void Start()
    {
        foreach (GameObject obj in transitionWhenStart)
        {
            if (obj != null)
            {
                originalScales[obj] = obj.transform.localScale;
                obj.transform.localScale = new Vector3(InitialScale, InitialScale, InitialScale);
                objectStartTimes[obj] = 0f;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (First)
        {
            if (other.CompareTag("Player") && !isScaling)
            {
                isScaling = true;
                triggerTime = Time.time;

                // 初始化每个物体的开始时间
                for (int i = 0; i < transitionWhenStart.Count; i++)
                {
                    GameObject obj = transitionWhenStart[i];
                    if (scaleOneByOne)
                    {
                        // 如果是逐个缩放，则根据索引设置延迟时间
                        objectStartTimes[obj] = triggerTime + (i * OneByOneTime);
                    }
                    else
                    {
                        // 如果是同时缩放，则所有物体的开始时间相同
                        objectStartTimes[obj] = triggerTime;
                    }
                }
                First = false;
            }
        }
    }

    void Update()
    {
        if (isScaling)
        {
            bool allComplete = true;
            float currentTime = Time.time;

            foreach (GameObject obj in transitionWhenStart)
            {
                if (obj != null)
                {
                    // 检查该物体是否应该开始缩放
                    if (currentTime >= objectStartTimes[obj])
                    {
                        float objectTime = currentTime - objectStartTimes[obj];
                        float t = Mathf.Clamp01(objectTime / scaleTime);
                        float smoothT = Mathf.SmoothStep(0f, 1f, t);

                        Vector3 targetScale = originalScales[obj];
                        Vector3 initialScale = new Vector3(InitialScale, InitialScale, InitialScale);
                        obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, smoothT);

                        // 检查这个物体是否完成缩放
                        if (t < 1f)
                        {
                            allComplete = false;
                        }
                    }
                    else
                    {
                        allComplete = false;
                    }
                }
            }

            // 所有物体都完成缩放后，结束缩放状态
            if (allComplete)
            {
                isScaling = false;
            }
        }
    }
}