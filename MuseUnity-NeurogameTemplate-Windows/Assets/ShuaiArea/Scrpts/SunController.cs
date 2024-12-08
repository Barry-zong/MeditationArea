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

    // ��������̫��״̬�Ĳ���
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

            // ���ó�ʼ״̬��״̬һ��
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
                // ʹ��focusֵӳ�����ǿ��
                float mappedIntensity = Mathf.Lerp(minIntensity, maxIntensity,
                    Mathf.Clamp(InteraxonInterfacer.Instance.focus, 0f, 1f));
                sunLight.intensity = mappedIntensity;
            }
        }
    }

    private void UpdateSunRotation()
    {
        // ��flowֵӳ�䵽��ת�Ƕ�
        float normalizedFlow = Mathf.InverseLerp(flowMin, flowMax, InteraxonInterfacer.Instance.flow);
        float rotationX = Mathf.Lerp(minRotationX, maxRotationX, normalizedFlow);

        // ����ԭ�е�y��z����תֵ
        Vector3 currentRotation = directionalLight.transform.eulerAngles;
        directionalLight.transform.eulerAngles = new Vector3(rotationX, currentRotation.y, currentRotation.z);
    }

    private void OnTriggerExit(Collider other)
    {
        // ����Ƿ�������뿪��δ�ı��״̬
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