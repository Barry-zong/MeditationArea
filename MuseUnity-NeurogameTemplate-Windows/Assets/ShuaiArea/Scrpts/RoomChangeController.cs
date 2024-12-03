using UnityEngine;

public class RoomChangeController : MonoBehaviour
{
    public GameObject moonSize;         // ������С������
    public GameObject originRoom;       // ԭʼ����
    public GameObject changeRoom;       // Ҫ�ı�λ�õķ���
    public float targetScale = 1.5f;    // ����λ�øı��Ŀ���С
    public Collider triggerZone;       // ����������ײ��

    public bool triggerActivated = false;  // ����Ƿ��Ѿ���������

    private void Start()
    {
        if (triggerZone != null)
        {
            // ȷ�� triggerZone �Ǵ�����
            triggerZone.isTrigger = true;
        }
        else
        {
            Debug.LogError("triggerZoneδ��ָ����");
        }
    }
    void OnEnable()
    {
        // ע���¼�����
        StartCoroutine(CheckTriggerZone());
    }

    private System.Collections.IEnumerator CheckTriggerZone()
    {
        while (enabled)
        {
            if (triggerZone != null)
            {
                // ����Ƿ���Player�ڴ���������
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
                        yield break; // ������ֹͣ���
                    }
                }
            }
            yield return new WaitForSeconds(0.1f); // ÿ0.1����һ��
        }
    }

    private void Update()
    {
        // ֻҪ�����������ͳ������������С
        if (triggerActivated)
        {
            Debug.Log("0");
            // ��� moonSize ���������ֵ�Ƿ�ﵽĿ���С
            if (moonSize != null && moonSize.transform.localScale.x >= targetScale)
            {
                Debug.Log("1");
                // ����������䶼����
                if (originRoom != null && changeRoom != null)
                {
                    Debug.Log("2");
                    // ֱ���޸ĵڶ��������Y��λ��
                    Vector3 roomPosition = changeRoom.transform.position;
                    roomPosition.y -= 20f;
                    changeRoom.transform.position = roomPosition;
                    // ɾ��ԭʼ����
                    Destroy(originRoom);
                }
            }
        }
    }
}