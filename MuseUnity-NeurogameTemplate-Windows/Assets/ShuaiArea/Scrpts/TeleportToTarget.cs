using UnityEngine;

public class TeleportToTarget : MonoBehaviour
{
    // Ŀ��λ�ö��������
    public GameObject TargetPosition;
    public FlowRoomSwitch RoomSwitch;
    public RandomRotatingBalls randomRotatingballs;
    public RandomRotatingBalls randomRotatingballs2;
    public RandomRotatingBalls randomRotatingballs3;

    private void OnTriggerEnter(Collider other)
    {
        // �����봥�����Ķ����Ƿ����"Player"��ǩ
        if (other.CompareTag("Player"))
        {
            // ��ȡ��ǰ�������ת
            Quaternion currentRotation = other.transform.rotation;

            // ������λ������ΪĿ��λ�ã�����ԭ����ת
            other.transform.position = TargetPosition.transform.position;
            other.transform.rotation = currentRotation;
            RoomSwitch.flowStartBool = false;
            randomRotatingballs.FinalSceneStart = true;
            randomRotatingballs2.FinalSceneStart = true;
            randomRotatingballs3.FinalSceneStart = true;
        }
    }
}