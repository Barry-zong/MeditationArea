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
    public float rotationSmoothSpeed = 2f; // ��תƽ���ٶ�

    private readonly float minRotationX = -300f;
    private readonly float maxRotationX = -240f;
    private Light sunLight;
    private bool hasChangedState = false;
    private bool isStateTwo = false;

    // ��������̫��״̬�Ĳ���
    private readonly Color stateOneEmission = new Color(0.973f, 0.961f, 0.878f);
    private readonly Color stateTwoEmission = new Color(0.404f, 0.671f, 1f);
    private readonly float stateOneIntensity = 0.8f;
    private readonly float stateTwoIntensity = 0.02f;

    // ���ڹ��ɶ����ı���
    private float transitionStartTime;
    private bool isTransitioning = false;
    private Color startEmission;
    private Color targetEmission;
    private float startIntensity;
    private float targetIntensity;

    // ��ת��ر���
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Start()
    {
        if (directionalLight != null)
        {
            sunLight = directionalLight.GetComponent<Light>();
            directionalLight.SetActive(true);
            SetSunState(stateOneEmission, stateOneIntensity, false);

            // ��ʼ����תֵ
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
        // ����Ŀ����תֵ
        float normalizedFlow = Mathf.InverseLerp(flowMin, flowMax, InteraxonInterfacer.Instance.flow);
        float targetRotationX = Mathf.Lerp(minRotationX, maxRotationX, normalizedFlow);
        targetRotation = new Vector3(targetRotationX, currentRotation.y, currentRotation.z);

        // ƽ�����ɵ�Ŀ����תֵ
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);

        // Ӧ����ת
        directionalLight.transform.eulerAngles = currentRotation;

        // ����ǶȻ��ƣ�ȷ���Ƕ��ں���Χ�ڣ�
        currentRotation = NormalizeRotation(currentRotation);
    }

    private Vector3 NormalizeRotation(Vector3 rotation)
    {
        // ȷ���Ƕ��� -360 �� 360 ��֮��
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