using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    [Header("基础设置")]
    [Tooltip("基础浮动高度")]
    public float baseFloatHeight = 1.0f;

    [Tooltip("基础浮动速度")]
    public float baseFloatSpeed = 1.0f;

    [Header("随机设置")]
    [Tooltip("高度随机变化范围(0-1之间)")]
    [Range(0f, 1f)]
    public float heightRandomness = 0.2f;

    [Tooltip("速度随机变化范围(0-1之间)")]
    [Range(0f, 1f)]
    public float speedRandomness = 0.2f;

    [Header("平滑设置")]
    [Tooltip("平滑度(值越小越平滑)")]
    [Range(0.1f, 10f)]
    public float smoothness = 1.0f;

    [Header("专注度设置")]
    [Tooltip("触发浮动的专注度阈值")]
    public float focusThreshold = 0.1f;

    [Tooltip("返回初始位置的速度")]
    public float returnSpeed = 2.0f;

    // 私有变量
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
        // 记录初始位置
        startY = transform.position.y;
        initialPosition = transform.position;

        // 初始化随机值
        InitializeRandomValues();

        // 设置第一个目标位置
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
            // 如果之前没有在浮动，重新初始化随机值
            if (!wasFloating)
            {
                InitializeRandomValues();
                wasFloating = true;
            }

            // 正常的浮动逻辑
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
            // 如果不满足浮动条件，平滑返回初始位置
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

            // 绘制初始位置
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(initialPosition, 0.15f);
        }
    }
}