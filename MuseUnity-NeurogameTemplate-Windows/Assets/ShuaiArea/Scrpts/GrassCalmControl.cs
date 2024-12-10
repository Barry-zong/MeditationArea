using UnityEngine;

public class GrassCalmControl : MonoBehaviour
{
    public GameObject GrassPlane;
    public float GrassIntensity = 0;
    public Material terrainGrassMaterial;

    [Header("Transition Settings")]
    [Range(0.1f, 10f)]
    public float smoothSpeed = 2f; // ƽ�������ٶ�

    private float calmValue = 0;
    private float currentIntensity = 0; // ��ǰʵ��ǿ��ֵ
    private float targetIntensity = 0;  // Ŀ��ǿ��ֵ
    private float currentWindStrength = 1f; // ��ǰ��ǿ��
    private float targetWindStrength = 1f;  // Ŀ���ǿ��
    private Vector3 targetScale;     // Ŀ������ֵ
    private Vector3 currentScale;    // ��ǰʵ������ֵ

    void Start()
    {
        if (GrassPlane != null)
        {
            currentScale = GrassPlane.transform.localScale;
            targetScale = currentScale;
        }
    }

    void Update()
    {
        // ����Ŀ��ֵ
        UpdateTargetValues();

        // ƽ�����ɵ�Ŀ��ֵ
        SmoothTransition();

        // Ӧ�ñ仯
        ApplyChanges();
    }

    private void UpdateTargetValues()
    {
        // ����Ŀ��ǿ��
        targetIntensity = Mathf.Min(InteraxonInterfacer.Instance.calm, 1.5f);

        // ����Ŀ��calmֵ
        float targetCalmValue = Mathf.InverseLerp(0, 1.5f, targetIntensity);

        // ����Ŀ������
        targetScale = new Vector3(
            GrassPlane.transform.localScale.x,
            1 + targetIntensity / 3,
            GrassPlane.transform.localScale.z
        );

        // ����Ŀ���ǿ��
        targetWindStrength = 1 - targetCalmValue;
    }

    private void SmoothTransition()
    {
        // ƽ�����ɵ�ǰǿ��
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * smoothSpeed);

        // ƽ�����ɵ�ǰ����
        currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * smoothSpeed);

        // ƽ�����ɵ�ǰ��ǿ��
        currentWindStrength = Mathf.Lerp(currentWindStrength, targetWindStrength, Time.deltaTime * smoothSpeed);

        // ����calmֵ
        calmValue = Mathf.InverseLerp(0, 1.5f, currentIntensity);
    }

    private void ApplyChanges()
    {
        // Ӧ������
        if (GrassPlane != null)
        {
            GrassPlane.transform.localScale = currentScale;
        }

        // Ӧ�÷�ǿ��
        if (terrainGrassMaterial != null)
        {
            terrainGrassMaterial.SetFloat("_WindStrength", currentWindStrength);
        }

        // ���¹�����GrassIntensityֵ
        GrassIntensity = currentIntensity;
    }

    // ��ѡ���ṩ�ֶ����·�ǿ�ȵķ���
    public void UpdateWindStrength(float strength)
    {
        targetWindStrength = strength;
    }
}