using UnityEngine;

public class SunController : MonoBehaviour
{
    public GameObject directionalLight;

    [Header("Intensity Mapping")]
    [Range(0f, 2f)]
    public float minIntensity = 0.8f;
    [Range(0f, 2f)]
    public float maxIntensity = 1.2f;

    [Header("Rotation Mapping")]
    [Range(0f, 2f)]
    public float flowMin = 0f;
    [Range(0f, 2f)]
    public float flowMax = 2f;

    [Header("Transition Settings")]
    [Range(0.1f, 5f)]
    public float transitionDuration = 2f;
    [Range(0.1f, 5f)]
    public float rotationSmoothSpeed = 2f; // 旋转平滑速度

    private readonly float minRotationX = -300f;
    private readonly float maxRotationX = -240f;
    private Light sunLight;
    private bool hasChangedState = false;
    private bool isStateTwo = false;

    // 定义两种太阳状态的参数
    private readonly Color stateOneEmission = new Color(0.973f, 0.961f, 0.878f);
    private readonly Color stateTwoEmission = new Color(0.404f, 0.671f, 1f);
    private readonly float stateOneIntensity = 0.8f;
    private readonly float stateTwoIntensity = 0.02f;

    // 用于过渡动画的变量
    private float transitionStartTime;
    private bool isTransitioning = false;
    private Color startEmission;
    private Color targetEmission;
    private float startIntensity;
    private float targetIntensity;

    // 旋转相关变量
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Start()
    {
        if (directionalLight != null)
        {
            sunLight = directionalLight.GetComponent<Light>();
            directionalLight.SetActive(true);
            SetSunState(stateOneEmission, stateOneIntensity, false);

            // 初始化旋转值
            currentRotation = directionalLight.transform.eulerAngles;
            targetRotation = currentRotation;
        }
    }

    private void Update()
    {
        if (sunLight != null && InteraxonInterfacer.Instance != null)
        {
            UpdateSunRotation();

            if (isTransitioning)
            {
                UpdateTransition();
            }
            else if (!isStateTwo)
            {
                float mappedIntensity = Mathf.Lerp(minIntensity, maxIntensity,
                    Mathf.Clamp(InteraxonInterfacer.Instance.focus, 0f, 1f));
                sunLight.intensity = mappedIntensity;
            }
        }
    }

    private void UpdateSunRotation()
    {
        // 计算目标旋转值
        float normalizedFlow = Mathf.InverseLerp(flowMin, flowMax, InteraxonInterfacer.Instance.flow);
        float targetRotationX = Mathf.Lerp(minRotationX, maxRotationX, normalizedFlow);
        targetRotation = new Vector3(targetRotationX, currentRotation.y, currentRotation.z);

        // 平滑过渡到目标旋转值
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);

        // 应用旋转
        directionalLight.transform.eulerAngles = currentRotation;

        // 处理角度环绕（确保角度在合理范围内）
        currentRotation = NormalizeRotation(currentRotation);
    }

    private Vector3 NormalizeRotation(Vector3 rotation)
    {
        // 确保角度在 -360 到 360 度之间
        rotation.x = NormalizeAngle(rotation.x);
        rotation.y = NormalizeAngle(rotation.y);
        rotation.z = NormalizeAngle(rotation.z);
        return rotation;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 360f)
            angle -= 360f;
        while (angle < -360f)
            angle += 360f;
        return angle;
    }

    private void UpdateTransition()
    {
        float timeSinceStart = Time.time - transitionStartTime;
        float progress = Mathf.Clamp01(timeSinceStart / transitionDuration);

        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

        sunLight.color = Color.Lerp(startEmission, targetEmission, smoothProgress);
        sunLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, smoothProgress);

        if (progress >= 1f)
        {
            isTransitioning = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasChangedState)
        {
            SetSunState(stateTwoEmission, stateTwoIntensity, true);
            hasChangedState = true;
            isStateTwo = true;
        }
    }

    private void SetSunState(Color emission, float intensity, bool useTransition)
    {
        if (sunLight == null) return;

        if (useTransition)
        {
            startEmission = sunLight.color;
            targetEmission = emission;
            startIntensity = sunLight.intensity;
            targetIntensity = intensity;
            transitionStartTime = Time.time;
            isTransitioning = true;
        }
        else
        {
            sunLight.color = emission;
            sunLight.intensity = intensity;
        }
    }
}