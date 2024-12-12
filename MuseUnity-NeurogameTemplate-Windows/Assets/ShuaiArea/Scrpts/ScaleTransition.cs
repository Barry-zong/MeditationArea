using UnityEngine;
using System.Collections.Generic;

public class ScaleTransition : MonoBehaviour
{
    [Tooltip("��Ҫ�������Ź��ɵ���Ϸ�����б�")]
    public List<GameObject> transitionWhenStart = new List<GameObject>();

    [Tooltip("��ʼ����ֵ")]
    public float InitialScale = 0f;

    [Tooltip("�����������ʱ��(��)")]
    public float scaleTime = 1.0f;

    [Tooltip("�Ƿ��������")]
    public bool scaleOneByOne = false;

    [Tooltip("������ŵ�ʱ����")]
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

                // ��ʼ��ÿ������Ŀ�ʼʱ��
                for (int i = 0; i < transitionWhenStart.Count; i++)
                {
                    GameObject obj = transitionWhenStart[i];
                    if (scaleOneByOne)
                    {
                        // �����������ţ���������������ӳ�ʱ��
                        objectStartTimes[obj] = triggerTime + (i * OneByOneTime);
                    }
                    else
                    {
                        // �����ͬʱ���ţ�����������Ŀ�ʼʱ����ͬ
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
                    // ���������Ƿ�Ӧ�ÿ�ʼ����
                    if (currentTime >= objectStartTimes[obj])
                    {
                        float objectTime = currentTime - objectStartTimes[obj];
                        float t = Mathf.Clamp01(objectTime / scaleTime);
                        float smoothT = Mathf.SmoothStep(0f, 1f, t);

                        Vector3 targetScale = originalScales[obj];
                        Vector3 initialScale = new Vector3(InitialScale, InitialScale, InitialScale);
                        obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, smoothT);

                        // �����������Ƿ��������
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

            // �������嶼������ź󣬽�������״̬
            if (allComplete)
            {
                isScaling = false;
            }
        }
    }
}