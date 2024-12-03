using UnityEngine;

public class RoomChangeController : MonoBehaviour
{
    public GameObject moonSize;         // 月亮大小的物体
    public GameObject originRoom;       // 原始房间
    public GameObject changeRoom;       // 要改变位置的房间
    public float targetScale = 1.5f;    // 触发位置改变的目标大小
    public Collider triggerZone;       // 触发区域碰撞体

    public bool triggerActivated = false;  // 检测是否已经被触发过

    private void Start()
    {
        if (triggerZone != null)
        {
            // 确保 triggerZone 是触发器
            triggerZone.isTrigger = true;
        }
        else
        {
            Debug.LogError("triggerZone未被指定！");
        }
    }
    void OnEnable()
    {
        // 注册事件监听
        StartCoroutine(CheckTriggerZone());
    }

    private System.Collections.IEnumerator CheckTriggerZone()
    {
        while (enabled)
        {
            if (triggerZone != null)
            {
                // 检查是否有Player在触发区域内
                Collider[] colliders = Physics.OverlapBox(
                    triggerZone.bounds.center,
                    triggerZone.bounds.extents,
                    triggerZone.transform.rotation
                );

                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("Player"))
                    {
                        triggerActivated = true;
                        yield break; // 触发后停止检查
                    }
                }
            }
            yield return new WaitForSeconds(0.1f); // 每0.1秒检查一次
        }
    }

    private void Update()
    {
        // 只要曾经触发过就持续检查月亮大小
        if (triggerActivated)
        {
            Debug.Log("0");
            // 检查 moonSize 物体的缩放值是否达到目标大小
            if (moonSize != null && moonSize.transform.localScale.x >= targetScale)
            {
                Debug.Log("1");
                // 如果两个房间都存在
                if (originRoom != null && changeRoom != null)
                {
                    Debug.Log("2");
                    // 直接修改第二个房间的Y轴位置
                    Vector3 roomPosition = changeRoom.transform.position;
                    roomPosition.y -= 20f;
                    changeRoom.transform.position = roomPosition;
                    // 删除原始房间
                    Destroy(originRoom);
                }
            }
        }
    }
}