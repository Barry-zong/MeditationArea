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

    private readonly float minRotationX = -300f;
    private readonly float maxRotationX = -240f;

    private Light sunLight;
    private bool hasChangedState = false;
    private bool isStateTwo = false;

    // 定义两种太阳状态的参数
    private readonly Color stateOneEmission = new Color(0.973f, 0.961f, 0.878f); // F8F5E0
    private readonly Color stateTwoEmission = new Color(0.404f, 0.671f, 1f);    // 67ABFF
    private readonly float stateOneIntensity = 0.8f;
    private readonly float stateTwoIntensity = 0.02f;

    private void Start()
    {
        if (directionalLight != null)
        {
            sunLight = directionalLight.GetComponent<Light>();
            directionalLight.SetActive(true);

            // 设置初始状态（状态一）
            SetSunState(stateOneEmission, stateOneIntensity);
        }
    }

    private void Update()
    {
        if (sunLight != null && InteraxonInterfacer.Instance != null)
        {
            UpdateSunRotation();

            if (!isStateTwo)
            {
                // 使用focus值映射光照强度
                float mappedIntensity = Mathf.Lerp(minIntensity, maxIntensity,
                    Mathf.Clamp(InteraxonInterfacer.Instance.focus, 0f, 1f));
                sunLight.intensity = mappedIntensity;
            }
        }
    }

    private void UpdateSunRotation()
    {
        // 将flow值映射到旋转角度
        float normalizedFlow = Mathf.InverseLerp(flowMin, flowMax, InteraxonInterfacer.Instance.flow);
        float rotationX = Mathf.Lerp(minRotationX, maxRotationX, normalizedFlow);

        // 保持原有的y和z轴旋转值
        Vector3 currentRotation = directionalLight.transform.eulerAngles;
        directionalLight.transform.eulerAngles = new Vector3(rotationX, currentRotation.y, currentRotation.z);
    }

    private void OnTriggerExit(Collider other)
    {
        // 检查是否是玩家离开且未改变过状态
        if (other.CompareTag("Player") && !hasChangedState)
        {
            SetSunState(stateTwoEmission, stateTwoIntensity);
            hasChangedState = true;
            isStateTwo = true;
        }
    }

    private void SetSunState(Color emission, float intensity)
    {
        if (sunLight != null)
        {
            sunLight.color = emission;
            sunLight.intensity = intensity;
        }
    }
}