using UnityEngine;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour
{
    public float a = 0.2f;  // ��Χ0-0.3
    public float b = 0.1f;  // ��Χ0-0.3

    private float c;

    // ��Ϸ�����б�
    public List<GameObject> controlledObjects = new List<GameObject>();

    // ���ڴ洢����ĳ�ʼ�任��Ϣ
    private Dictionary<GameObject, TransformData> initialTransforms = new Dictionary<GameObject, TransformData>();

    // ���ڴ洢��ʼ�任���ݵ���
    private class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformData(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
            scale = transform.localScale;
        }
    }

    void Start()
    {
        // ��¼��������ĳ�ʼ�任��Ϣ
        foreach (GameObject obj in controlledObjects)
        {
            if (obj != null && !initialTransforms.ContainsKey(obj))
            {
                initialTransforms.Add(obj, new TransformData(obj.transform));
            }
        }
    }

    void Update()

    {
        b = InteraxonInterfacer.Instance.focus;
        a = InteraxonInterfacer.Instance.calm;
        // ����cֵ
        c = Mathf.Clamp((a + b) / 2, 0f, 0.3f);

        foreach (GameObject obj in controlledObjects)
        {
            if (obj != null && initialTransforms.ContainsKey(obj))
            {
                TransformData initialData = initialTransforms[obj];

                // ���ڳ�ʼ��תӦ���µ���ת
                Quaternion additionalRotation = Quaternion.Euler(
                    b * 360 * Time.deltaTime,  // X����ת
                    a * 360 * Time.deltaTime,  // Y����ת
                    0                          // Z����ת
                );
                obj.transform.rotation = obj.transform.rotation * additionalRotation;

                // ���ڳ�ʼ����Ӧ���µ�����
                float scaleMultiplier = 1 + Mathf.Sin(Time.time) * c;
                obj.transform.localScale = new Vector3(
                    initialData.scale.x * scaleMultiplier,
                    initialData.scale.y * scaleMultiplier,
                    initialData.scale.z * scaleMultiplier
                );
            }
        }
    }

    void OnValidate()
    {
        a = Mathf.Clamp(a, 0f, 0.3f);
        b = Mathf.Clamp(b, 0f, 0.3f);
    }

    // ���������ʱ��¼���ʼ�任��Ϣ
    public void AddObject(GameObject obj)
    {
        if (!controlledObjects.Contains(obj))
        {
            controlledObjects.Add(obj);
            initialTransforms.Add(obj, new TransformData(obj.transform));
        }
    }

    // �Ƴ�����ʱͬʱ�Ƴ����ʼ�任��Ϣ
    public void RemoveObject(GameObject obj)
    {
        controlledObjects.Remove(obj);
        initialTransforms.Remove(obj);
    }

    // �������嵽��ʼ״̬
    public void ResetObject(GameObject obj)
    {
        if (initialTransforms.ContainsKey(obj))
        {
            TransformData initialData = initialTransforms[obj];
            obj.transform.position = initialData.position;
            obj.transform.rotation = initialData.rotation;
            obj.transform.localScale = initialData.scale;
        }
    }

    // ������������
    public void ResetAllObjects()
    {
        foreach (GameObject obj in controlledObjects)
        {
            if (obj != null)
            {
                ResetObject(obj);
            }
        }
    }
}