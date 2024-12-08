using UnityEngine;
using UnityEngine.Playables;
using Interaxon.Libmuse;

public class FlowerControl : MonoBehaviour
{
    [Header("Components")]
    public PlayableDirector director;
    public GameObject RotateItem;

    [Header("Animation Settings")]
    [Range(0f, 10f)]
    public float flowerMove = 7f;    // ���ٶ�����ֵ
    [Range(0f, 10f)]
    public float flowerSlow = 4f;    // ���ٶ�����ֵ
    [Range(0f, 10f)]
    public float flowerStop = 2f;    // ֹͣ������ֵ

    [Header("Rotation Settings")]
    public float rotateSpeed = 90f;   // ������ת�ٶ�
    public float transitionSpeed = 2f; // ״̬�����ٶ�

    // ˽�б���
    private float currentAnimSpeed = 1f;  // ��ǰ�����ٶ�
    private float targetAnimSpeed = 1f;   // Ŀ�궯���ٶ�
    private float currentRotateSpeed = 0f;// ��ǰʵ����ת�ٶ�

    void Update()
    {
        if (InteraxonInterfacer.Instance.currentConnectionState != ConnectionState.CONNECTED)
        {
            // ���δ���ӣ�����ֹͣ���ж���
            targetAnimSpeed = 0f;
            currentRotateSpeed = 0f;
        }
        else
        {
            // ��ȡ������flowֵ
            float MuseNumber_flower = Mathf.Clamp(InteraxonInterfacer.Instance.focus * 10, 0, 10);

            // ����flowֵȷ��Ŀ�궯���ٶ�
            if (MuseNumber_flower > flowerMove)
            {
                targetAnimSpeed = 2.0f;  // ����״̬
                currentRotateSpeed = 0f;  // ����ת
            }
            else if (MuseNumber_flower > flowerSlow)
            {
                targetAnimSpeed = 0.5f;   // ����״̬
                currentRotateSpeed = 0f;  // ����ת
            }
            else if (MuseNumber_flower > flowerStop)
            {
                targetAnimSpeed = 0f;     // ֹͣ����
                currentRotateSpeed = rotateSpeed;  // ��ʼ��ת
            }
            else
            {
                targetAnimSpeed = 0f;     // ��ȫֹͣ
                currentRotateSpeed = 0f;  // ����ת
            }
        }

        // ƽ�����ɶ����ٶ�
        currentAnimSpeed = Mathf.Lerp(currentAnimSpeed, targetAnimSpeed, Time.deltaTime * transitionSpeed);
        director.playableGraph.GetRootPlayable(0).SetSpeed(currentAnimSpeed);

        // Ӧ����ת
        if (RotateItem != null && currentRotateSpeed > 0)
        {
            RotateItem.transform.Rotate(Vector3.up * currentRotateSpeed * Time.deltaTime, Space.Self);
        }
    }

    // ��Unity�༭������֤���õ���ֵ
    void OnValidate()
    {
        // ȷ����ֵ�Ĵ�С��ϵ��ȷ
        flowerMove = Mathf.Max(flowerMove, flowerSlow + 0.1f);
        flowerSlow = Mathf.Max(flowerSlow, flowerStop + 0.1f);
    }
}