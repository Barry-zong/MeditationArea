using UnityEngine;

public class StartDoorOpenControl : MonoBehaviour
{
    public bool DoorTest = false;
    // ���������ŵ���Ϸ����
    public GameObject door1;
    public GameObject door2;

    // �������ƶ��ľ���
    public float doorMoveDistance = 2f;

    // �������ƶ����ٶ�
    public float doorSpeed = 2f;

    // ����׷�����Ƿ������ƶ�
    private bool isMoving = false;

    // ��¼�ŵĳ�ʼλ��
    private Vector3 door1StartPos;
    private Vector3 door2StartPos;

    // Start is called before the first frame update
    void Start()
    {
        // �����ŵĳ�ʼλ��
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
        // ��������ƶ�״̬
        if (isMoving)
        {
            // ����door1����λ�ã������ƶ���
            if (door1 != null)
            {
                float newX = door1.transform.position.x - (doorSpeed * Time.deltaTime);
                // ȷ������������ƶ�����
                if (door1StartPos.x - newX <= doorMoveDistance)
                {
                    door1.transform.position = new Vector3(newX,
                                                         door1.transform.position.y,
                                                         door1.transform.position.z);
                }
            }

            // ����door2����λ�ã������ƶ���
            if (door2 != null)
            {
                float newX = door2.transform.position.x + (doorSpeed * Time.deltaTime);
                // ȷ������������ƶ�����
                if (newX - door2StartPos.x <= doorMoveDistance)
                {
                    door2.transform.position = new Vector3(newX,
                                                         door2.transform.position.y,
                                                         door2.transform.position.z);
                }
            }
        }
    }

    // ��ʼ�ŵ��ƶ�
    public void StartDoorMovement()
    {
        isMoving = true;
    }

    // ֹͣ�ŵ��ƶ�
    public void StopDoorMovement()
    {
        isMoving = false;
    }

    // �����ŵ�λ��
    public void ResetDoors()
    {
        if (door1 != null) door1.transform.position = door1StartPos;
        if (door2 != null) door2.transform.position = door2StartPos;
        isMoving = false;
    }
}