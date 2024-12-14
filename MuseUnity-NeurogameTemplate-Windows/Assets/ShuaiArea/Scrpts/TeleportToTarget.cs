using UnityEngine;

public class TeleportToTarget : MonoBehaviour
{
    // 目标位置对象的引用
    public GameObject TargetPosition;
    public FlowRoomSwitch RoomSwitch;
    public RandomRotatingBalls randomRotatingballs;
    public RandomRotatingBalls randomRotatingballs2;
    public RandomRotatingBalls randomRotatingballs3;

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入触发器的对象是否带有"Player"标签
        if (other.CompareTag("Player"))
        {
            // 获取当前物体的旋转
            Quaternion currentRotation = other.transform.rotation;

            // 将物体位置设置为目标位置，保持原有旋转
            other.transform.position = TargetPosition.transform.position;
            other.transform.rotation = currentRotation;
            RoomSwitch.flowStartBool = false;
            randomRotatingballs.FinalSceneStart = true;
            randomRotatingballs2.FinalSceneStart = true;
            randomRotatingballs3.FinalSceneStart = true;
        }
    }
}