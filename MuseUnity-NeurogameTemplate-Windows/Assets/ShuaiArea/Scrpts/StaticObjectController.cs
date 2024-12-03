using UnityEngine;

public class StaticObjectController : MonoBehaviour
{
    public GameObject[] controlledObjects;

    void Start()
    {
        if (controlledObjects == null || controlledObjects.Length == 0)
        {
            Debug.LogWarning("û��ָ��Ҫ���Ƶ����壡");
            return;
        }

        foreach (GameObject obj in controlledObjects)
        {
            if (obj != null)
            {
                // ȷ����������ƶ�
                obj.isStatic = false;
                Debug.Log($"�ѽ� {obj.name} ����Ϊ�Ǿ�̬");
            }
        }
    }
}