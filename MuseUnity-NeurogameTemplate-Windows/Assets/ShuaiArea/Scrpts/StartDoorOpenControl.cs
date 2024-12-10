using UnityEngine;

public class StartDoorOpenControl : MonoBehaviour
{
    public bool DoorTest = false;
    // 声明两扇门的游戏物体
    public GameObject door1;
    public GameObject door2;

    // 声明门移动的距离
    public float doorMoveDistance = 2f;

    // 声明门移动的速度
    public float doorSpeed = 2f;

    // 用于追踪门是否正在移动
    private bool isMoving = false;

    // 记录门的初始位置
    private Vector3 door1StartPos;
    private Vector3 door2StartPos;

    // Start is called before the first frame update
    void Start()
    {
        // 保存门的初始位置
        if (door1 != null) door1StartPos = door1.transform.position;
        if (door2 != null) door2StartPos = door2.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(DoorTest || InteraxonInterfacer.Instance.calm > 0.1f || InteraxonInterfacer.Instance.flow > 0.1f || InteraxonInterfacer.Instance.focus > 0.1f)
        {
            StartDoorMovement();
        }
        // 如果门在移动状态
        if (isMoving)
        {
            // 计算door1的新位置（向左移动）
            if (door1 != null)
            {
                float newX = door1.transform.position.x - (doorSpeed * Time.deltaTime);
                // 确保不超过最大移动距离
                if (door1StartPos.x - newX <= doorMoveDistance)
                {
                    door1.transform.position = new Vector3(newX,
                                                         door1.transform.position.y,
                                                         door1.transform.position.z);
                }
            }

            // 计算door2的新位置（向右移动）
            if (door2 != null)
            {
                float newX = door2.transform.position.x + (doorSpeed * Time.deltaTime);
                // 确保不超过最大移动距离
                if (newX - door2StartPos.x <= doorMoveDistance)
                {
                    door2.transform.position = new Vector3(newX,
                                                         door2.transform.position.y,
                                                         door2.transform.position.z);
                }
            }
        }
    }

    // 开始门的移动
    public void StartDoorMovement()
    {
        isMoving = true;
    }

    // 停止门的移动
    public void StopDoorMovement()
    {
        isMoving = false;
    }

    // 重置门的位置
    public void ResetDoors()
    {
        if (door1 != null) door1.transform.position = door1StartPos;
        if (door2 != null) door2.transform.position = door2StartPos;
        isMoving = false;
    }
}