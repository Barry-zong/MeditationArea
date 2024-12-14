using UnityEngine;

public class CircularMotion : MonoBehaviour
{
    [Header("�˶�����")]
    [SerializeField] private float initialRadius = 1.72f;  // ��ʼԲ���˶��뾶
    private float currentRadius;      // ��ǰʵ�ʰ뾶
    public float speed = 2f;         // ��ת�ٶ�(����/��)
    public float height = 0f;        // Y��߶�
    public Vector3 rotationAxis = Vector3.up;  // ��ת��
    public Vector3 centerOffset = Vector3.zero; // Բ��ƫ����

    [Header("״̬����")]
    public bool isEnhanced = false;  // true: �ٶ�1/����3, false: �ٶ�0.3/����0.5
    public Material targetMaterial;   // Ŀ�������

    // ������ز���
    private Vector3 initialScale;     // ��ʼ����ֵ
    private Vector3 scaleAtThreshold; // �߶ȵ���2ʱ������ֵ
    private bool hasPassedThreshold = false;  // �Ƿ��Ѿ�����2�߶�
    private const float HEIGHT_START = 2f;    // ��ʼ���ŵĸ߶�
    private const float HEIGHT_END = 2.65f;   // �������ŵĸ߶�
    private const float MIN_SCALE = 0.1f;     // ��С���ű���

    private Vector3 centerPosition;    // Բ��λ��
    private float currentAngle = 0f;   // ��ǰ�Ƕ�
    private float targetSpeed;         // Ŀ���ٶ�
    private float initialSpeed;        // ��ʼ�ٶ�
    private float transitionTime;      // ����ʱ�������
    private const float TRANSITION_DURATION = 0.5f;  // ���ɳ���ʱ��

    private float currentEmission;     // ��ǰ�Է���ǿ��
    private float targetEmission;      // Ŀ���Է���ǿ��
    private bool isTransitioning = false;  // �Ƿ����ڹ���
    private bool lastIsEnhanced = false;   // ��¼��һ֡��״̬
    private Color baseEmissionColor;    // ����������ɫ
    private float baseEmissionIntensity; // ��������ǿ��

    private bool isRotating = false;   // �����Ƿ�ʼ��ת
    private bool hasTriggered = false; // ȷ��ֻ����һ��

    void Start()
    {
        // ��ʼ����ǰ�뾶
        currentRadius = initialRadius;

        // �����ʼ����ֵ
        initialScale = transform.localScale;
        scaleAtThreshold = initialScale;

        // ����Բ��λ�ã����Ǹ߶�ƫ��
        centerPosition = transform.position + centerOffset + Vector3.up * height;

        // ��ʼ��������ɫ��ǿ��
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

        // ��ʼ���ٶ�Ϊ0����ʼʱ����ת
        speed = 0f;
        isRotating = false;
    }

    void Update()
    {
        if (!isRotating) return;

        // ���״̬�仯����ʼ����
        CheckStateTransition();

        // �������
        if (isTransitioning)
        {
            HandleTransition();
        }

        // ���µ�ǰ�Ƕ�
        currentAngle += speed * Time.deltaTime;

        // �ֱ���°뾶������
        UpdateRadius();
        UpdateScale();

        // ������λ�ã�����߶�ƫ��
        Vector3 offset = new Vector3(
            Mathf.Sin(currentAngle) * currentRadius,
            height,
            Mathf.Cos(currentAngle) * currentRadius
        );

        // �����ת�᲻��Y�ᣬ��Ҫ����������ת
        if (rotationAxis != Vector3.up)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, rotationAxis);
            offset = rotation * offset;
        }

        // ��������λ��
        transform.position = centerPosition + offset;

        // ������һ֡��״̬
        lastIsEnhanced = isEnhanced;
    }

    private void UpdateRadius()
    {
        // �������߶ȷ�Χ��ƽ�����ɰ뾶
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
        // ����ճ�����ֵ�߶ȣ���¼��ǰ����ֵ
        if (height > HEIGHT_START && !hasPassedThreshold)
        {
            scaleAtThreshold = transform.localScale;
            hasPassedThreshold = true;
        }
        // ���������ֵ�߶ȣ����ñ��
        else if (height <= HEIGHT_START && hasPassedThreshold)
        {
            hasPassedThreshold = false;
            transform.localScale = initialScale;
            return;
        }

        // �ڸ߶ȷ�Χ�ڽ�������
        if (height > HEIGHT_START && height < HEIGHT_END)
        {
            float t = Mathf.InverseLerp(HEIGHT_START, HEIGHT_END, height);
            float scaleRatio = Mathf.Lerp(1f, MIN_SCALE, t);
            transform.localScale = scaleAtThreshold * scaleRatio;
        }
        // ����߶ȳ��������߶ȣ�������С����ֵ
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