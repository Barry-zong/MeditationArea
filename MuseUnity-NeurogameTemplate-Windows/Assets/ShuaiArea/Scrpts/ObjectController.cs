using UnityEngine;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour
{
    public float a = 0.2f;  // 范围0-0.3
    public float b = 0.1f;  // 范围0-0.3

    private float c;

    // 游戏对象列表
    public List<GameObject> controlledObjects = new List<GameObject>();

    // 用于存储物体的初始变换信息
    private Dictionary<GameObject, TransformData> initialTransforms = new Dictionary<GameObject, TransformData>();

    // 用于存储初始变换数据的类
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
        // 记录所有物体的初始变换信息
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
        // 计算c值
        c = Mathf.Clamp((a + b) / 2, 0f, 0.3f);

        foreach (GameObject obj in controlledObjects)
        {
            if (obj != null && initialTransforms.ContainsKey(obj))
            {
                TransformData initialData = initialTransforms[obj];

                // 基于初始旋转应用新的旋转
                Quaternion additionalRotation = Quaternion.Euler(
                    b * 360 * Time.deltaTime,  // X轴旋转
                    a * 360 * Time.deltaTime,  // Y轴旋转
                    0                          // Z轴旋转
                );
                obj.transform.rotation = obj.transform.rotation * additionalRotation;

                // 基于初始缩放应用新的缩放
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

    // 添加新物体时记录其初始变换信息
    public void AddObject(GameObject obj)
    {
        if (!controlledObjects.Contains(obj))
        {
            controlledObjects.Add(obj);
            initialTransforms.Add(obj, new TransformData(obj.transform));
        }
    }

    // 移除物体时同时移除其初始变换信息
    public void RemoveObject(GameObject obj)
    {
        controlledObjects.Remove(obj);
        initialTransforms.Remove(obj);
    }

    // 重置物体到初始状态
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

    // 重置所有物体
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