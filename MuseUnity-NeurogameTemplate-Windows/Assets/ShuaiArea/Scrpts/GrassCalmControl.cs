using UnityEngine;

public class GrassCalmControl : MonoBehaviour
{
    public GameObject GrassPlane;
    public float GrassIntensity = 0;
    public Material terrainGrassMaterial;

    [Header("Transition Settings")]
    [Range(0.1f, 10f)]
    public float smoothSpeed = 2f; // 平滑过渡速度

    private float calmValue = 0;
    private float currentIntensity = 0; // 当前实际强度值
    private float targetIntensity = 0;  // 目标强度值
    private float currentWindStrength = 1f; // 当前风强度
    private float targetWindStrength = 1f;  // 目标风强度
    private Vector3 targetScale;     // 目标缩放值
    private Vector3 currentScale;    // 当前实际缩放值

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
        // 更新目标值
        UpdateTargetValues();

        // 平滑过渡到目标值
        SmoothTransition();

        // 应用变化
        ApplyChanges();
    }

    private void UpdateTargetValues()
    {
        // 更新目标强度
        targetIntensity = Mathf.Min(InteraxonInterfacer.Instance.calm, 1.5f);

        // 计算目标calm值
        float targetCalmValue = Mathf.InverseLerp(0, 1.5f, targetIntensity);

        // 更新目标缩放
        targetScale = new Vector3(
            GrassPlane.transform.localScale.x,
            1 + targetIntensity / 3,
            GrassPlane.transform.localScale.z
        );

        // 更新目标风强度
        targetWindStrength = 1 - targetCalmValue;
    }

    private void SmoothTransition()
    {
        // 平滑过渡当前强度
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * smoothSpeed);

        // 平滑过渡当前缩放
        currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * smoothSpeed);

        // 平滑过渡当前风强度
        currentWindStrength = Mathf.Lerp(currentWindStrength, targetWindStrength, Time.deltaTime * smoothSpeed);

        // 更新calm值
        calmValue = Mathf.InverseLerp(0, 1.5f, currentIntensity);
    }

    private void ApplyChanges()
    {
        // 应用缩放
        if (GrassPlane != null)
        {
            GrassPlane.transform.localScale = currentScale;
        }

        // 应用风强度
        if (terrainGrassMaterial != null)
        {
            terrainGrassMaterial.SetFloat("_WindStrength", currentWindStrength);
        }

        // 更新公开的GrassIntensity值
        GrassIntensity = currentIntensity;
    }

    // 可选：提供手动更新风强度的方法
    public void UpdateWindStrength(float strength)
    {
        targetWindStrength = strength;
    }
}