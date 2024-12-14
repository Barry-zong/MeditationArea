using UnityEngine;

public class CircularMotion : MonoBehaviour
{
    [Header("运动参数")]
    [SerializeField] private float initialRadius = 1.72f;  // 初始圆周运动半径
    private float currentRadius;      // 当前实际半径
    public float speed = 2f;         // 旋转速度(弧度/秒)
    public float height = 0f;        // Y轴高度
    public Vector3 rotationAxis = Vector3.up;  // 旋转轴
    public Vector3 centerOffset = Vector3.zero; // 圆心偏移量

    [Header("状态控制")]
    public bool isEnhanced = false;  // true: 速度1/发光3, false: 速度0.3/发光0.5
    public Material targetMaterial;   // 目标材质球

    // 缩放相关参数
    private Vector3 initialScale;     // 初始缩放值
    private Vector3 scaleAtThreshold; // 高度到达2时的缩放值
    private bool hasPassedThreshold = false;  // 是否已经超过2高度
    private const float HEIGHT_START = 2f;    // 开始缩放的高度
    private const float HEIGHT_END = 2.65f;   // 结束缩放的高度
    private const float MIN_SCALE = 0.1f;     // 最小缩放比例

    private Vector3 centerPosition;    // 圆心位置
    private float currentAngle = 0f;   // 当前角度
    private float targetSpeed;         // 目标速度
    private float initialSpeed;        // 初始速度
    private float transitionTime;      // 过渡时间计数器
    private const float TRANSITION_DURATION = 0.5f;  // 过渡持续时间

    private float currentEmission;     // 当前自发光强度
    private float targetEmission;      // 目标自发光强度
    private bool isTransitioning = false;  // 是否正在过渡
    private bool lastIsEnhanced = false;   // 记录上一帧的状态
    private Color baseEmissionColor;    // 基础发光颜色
    private float baseEmissionIntensity; // 基础发光强度

    private bool isRotating = false;   // 控制是否开始旋转
    private bool hasTriggered = false; // 确保只触发一次

    void Start()
    {
        // 初始化当前半径
        currentRadius = initialRadius;

        // 保存初始缩放值
        initialScale = transform.localScale;
        scaleAtThreshold = initialScale;

        // 保存圆心位置，考虑高度偏移
        centerPosition = transform.position + centerOffset + Vector3.up * height;

        // 初始化发光颜色和强度
        if (targetMaterial != null)
        {
            baseEmissionColor = targetMaterial.GetColor("_EmissionColor");
            baseEmissionIntensity = baseEmissionColor.maxColorComponent;

            if (baseEmissionIntensity < 0.001f)
            {
                baseEmissionColor = Color.white;
                baseEmissionIntensity = 1.0f;
            }

            baseEmissionColor = new Color(
                baseEmissionColor.r / baseEmissionIntensity,
                baseEmissionColor.g / baseEmissionIntensity,
                baseEmissionColor.b / baseEmissionIntensity,
                baseEmissionColor.a
            );

            currentEmission = 0.5f;
            UpdateEmissionColor(currentEmission);
        }

        // 初始化速度为0，开始时不旋转
        speed = 0f;
        isRotating = false;
    }

    void Update()
    {
        if (!isRotating) return;

        // 检查状态变化并开始过渡
        CheckStateTransition();

        // 处理过渡
        if (isTransitioning)
        {
            HandleTransition();
        }

        // 更新当前角度
        currentAngle += speed * Time.deltaTime;

        // 分别更新半径和缩放
        UpdateRadius();
        UpdateScale();

        // 计算新位置，加入高度偏移
        Vector3 offset = new Vector3(
            Mathf.Sin(currentAngle) * currentRadius,
            height,
            Mathf.Cos(currentAngle) * currentRadius
        );

        // 如果旋转轴不是Y轴，需要进行轴向旋转
        if (rotationAxis != Vector3.up)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, rotationAxis);
            offset = rotation * offset;
        }

        // 更新物体位置
        transform.position = centerPosition + offset;

        // 更新上一帧的状态
        lastIsEnhanced = isEnhanced;
    }

    private void UpdateRadius()
    {
        // 在整个高度范围内平滑过渡半径
        if (height >= HEIGHT_END)
        {
            currentRadius = 0f;
        }
        else
        {
            float t = Mathf.InverseLerp(0f, HEIGHT_END, height);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            currentRadius = Mathf.Lerp(initialRadius, 0f, smoothT);
        }
    }

    private void UpdateScale()
    {
        // 如果刚超过阈值高度，记录当前缩放值
        if (height > HEIGHT_START && !hasPassedThreshold)
        {
            scaleAtThreshold = transform.localScale;
            hasPassedThreshold = true;
        }
        // 如果低于阈值高度，重置标记
        else if (height <= HEIGHT_START && hasPassedThreshold)
        {
            hasPassedThreshold = false;
            transform.localScale = initialScale;
            return;
        }

        // 在高度范围内进行缩放
        if (height > HEIGHT_START && height < HEIGHT_END)
        {
            float t = Mathf.InverseLerp(HEIGHT_START, HEIGHT_END, height);
            float scaleRatio = Mathf.Lerp(1f, MIN_SCALE, t);
            transform.localScale = scaleAtThreshold * scaleRatio;
        }
        // 如果高度超过结束高度，保持最小缩放值
        else if (height >= HEIGHT_END)
        {
            transform.localScale = scaleAtThreshold * MIN_SCALE;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            isRotating = true;
            speed = 0.3f;
        }
    }

    private void CheckStateTransition()
    {
        if (isEnhanced != lastIsEnhanced && !isTransitioning)
        {
            isTransitioning = true;
            transitionTime = 0f;
            initialSpeed = speed;

            if (isEnhanced)
            {
                targetSpeed = 1.5f;
                targetEmission = 3f;
            }
            else
            {
                targetSpeed = 0.3f;
                targetEmission = 0.5f;
            }
        }
    }

    private void HandleTransition()
    {
        transitionTime += Time.deltaTime;
        float t = transitionTime / TRANSITION_DURATION;

        if (t >= 1f)
        {
            t = 1f;
            isTransitioning = false;
            currentEmission = targetEmission;
        }

        speed = Mathf.Lerp(initialSpeed, targetSpeed, t);

        if (targetMaterial != null)
        {
            float newEmission = Mathf.Lerp(currentEmission, targetEmission, t);
            UpdateEmissionColor(newEmission);
        }
    }

    private void UpdateEmissionColor(float emissionIntensity)
    {
        Color newEmissionColor = baseEmissionColor * emissionIntensity;
        targetMaterial.EnableKeyword("_EMISSION");
        targetMaterial.SetColor("_EmissionColor", newEmissionColor);
    }

    void OnDrawGizmos()
    {
        Vector3 center = Application.isPlaying ? centerPosition : transform.position + centerOffset + Vector3.up * height;
        DrawCircle(center, Application.isPlaying ? currentRadius : initialRadius, rotationAxis);
    }

    void DrawCircle(Vector3 center, float radius, Vector3 axis)
    {
        Vector3 prevPos = center;
        int segments = 36;
        float angleStep = 2f * Mathf.PI / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 offset = new Vector3(
                Mathf.Sin(angle) * radius,
                0f,
                Mathf.Cos(angle) * radius
            );

            if (axis != Vector3.up)
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, axis);
                offset = rotation * offset;
            }

            Vector3 newPos = center + offset;
            if (i > 0)
            {
                Gizmos.DrawLine(prevPos, newPos);
            }
            prevPos = newPos;
        }
    }
}