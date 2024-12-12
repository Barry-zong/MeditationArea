using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("���������߶�")]
    public float baseFloatHeight = 1.0f;

    [Tooltip("���������ٶ�")]
    public float baseFloatSpeed = 1.0f;

    [Header("�������")]
    [Tooltip("�߶�����仯��Χ(0-1֮��)")]
    [Range(0f, 1f)]
    public float heightRandomness = 0.2f;

    [Tooltip("�ٶ�����仯��Χ(0-1֮��)")]
    [Range(0f, 1f)]
    public float speedRandomness = 0.2f;

    [Header("ƽ������")]
    [Tooltip("ƽ����(ֵԽСԽƽ��)")]
    [Range(0.1f, 10f)]
    public float smoothness = 1.0f;

    [Header("רע������")]
    [Tooltip("����������רע����ֵ")]
    public float focusThreshold = 0.1f;

    [Tooltip("���س�ʼλ�õ��ٶ�")]
    public float returnSpeed = 2.0f;

    // ˽�б���
    private float startY;
    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private bool movingUp = true;
    private float currentFloatHeight;
    private float currentFloatSpeed;
    private float nextRandomTime;
    private float randomTimeInterval;
    private bool wasFloating = true;

    void Start()
    {
        // ��¼��ʼλ��
        startY = transform.position.y;
        initialPosition = transform.position;

        // ��ʼ�����ֵ
        InitializeRandomValues();

        // ���õ�һ��Ŀ��λ��
        UpdateTargetPosition();
    }

    void InitializeRandomValues()
    {
        movingUp = Random.value > 0.5f;
        UpdateRandomValues();
        randomTimeInterval = Random.Range(2f, 4f);
        nextRandomTime = Time.time + randomTimeInterval;
    }

    void Update()
    {
        bool shouldFloat = InteraxonInterfacer.Instance.focus > focusThreshold;

        if (shouldFloat)
        {
            // ���֮ǰû���ڸ��������³�ʼ�����ֵ
            if (!wasFloating)
            {
                InitializeRandomValues();
                wasFloating = true;
            }

            // �����ĸ����߼�
            if (Time.time >= nextRandomTime)
            {
                UpdateRandomValues();
                nextRandomTime = Time.time + randomTimeInterval;
            }

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * currentFloatSpeed * smoothness
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                movingUp = !movingUp;
                UpdateTargetPosition();
            }
        }
        else
        {
            // ��������㸡��������ƽ�����س�ʼλ��
            wasFloating = false;
            transform.position = Vector3.Lerp(
                transform.position,
                initialPosition,
                Time.deltaTime * returnSpeed
            );
        }
    }

    void UpdateRandomValues()
    {
        float heightVariation = baseFloatHeight * heightRandomness;
        currentFloatHeight = baseFloatHeight + Random.Range(-heightVariation, heightVariation);

        float speedVariation = baseFloatSpeed * speedRandomness;
        currentFloatSpeed = baseFloatSpeed + Random.Range(-speedVariation, speedVariation);
    }

    void UpdateTargetPosition()
    {
        float targetHeight = movingUp ? currentFloatHeight : -currentFloatHeight;
        targetPosition = new Vector3(
            transform.position.x,
            startY + targetHeight,
            transform.position.z
        );
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPosition, 0.1f);

            Gizmos.color = Color.yellow;
            Vector3 topPoint = transform.position;
            topPoint.y = startY + currentFloatHeight;
            Vector3 bottomPoint = transform.position;
            bottomPoint.y = startY - currentFloatHeight;
            Gizmos.DrawLine(topPoint, bottomPoint);

            // ���Ƴ�ʼλ��
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(initialPosition, 0.15f);
        }
    }
}