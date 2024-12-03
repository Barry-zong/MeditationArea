using UnityEngine;

public class StaticObjectController : MonoBehaviour
{
    public GameObject[] controlledObjects;

    void Start()
    {
        if (controlledObjects == null || controlledObjects.Length == 0)
        {
            Debug.LogWarning("没有指定要控制的物体！");
            return;
        }

        foreach (GameObject obj in controlledObjects)
        {
            if (obj != null)
            {
                // 确保物体可以移动
                obj.isStatic = false;
                Debug.Log($"已将 {obj.name} 设置为非静态");
            }
        }
    }
}