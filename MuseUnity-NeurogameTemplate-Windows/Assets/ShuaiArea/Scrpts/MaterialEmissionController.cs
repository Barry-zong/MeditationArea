using UnityEngine;
using System.Collections;

public class MaterialEmissionController : MonoBehaviour
{
    public Material targetMaterial;  // 目标材质球
    public bool LighttheTree = false;  // 控制开关

    private readonly Color emissionColor = new Color(191f / 255f, 166f / 255f, 153f / 255f);  // RGB(191,166,153)
    private bool isAnimating = false;  // 动画状态标记
    private bool hasCompleted = false; // 标记是否完成过渡
    private float currentEmissionIntensity = -10f;  // 当前发光强度
    public AudioSource audioSource;

    void Start()
    {
        LighttheTree = false;
        hasCompleted = false;
        if (targetMaterial != null)
        {
            // 设置初始发光颜色和强度
            targetMaterial.EnableKeyword("_EMISSION");
            UpdateEmission(-10f);
        }
    }

    void Update()
    {
        // 只有在LighttheTree为true且动画未开始且未完成时才启动动画
        if (LighttheTree && !isAnimating && !hasCompleted)
        {
            audioSource.Play();
            Debug.Log("musicShouldPlay_tree");
            StartCoroutine(AnimateEmission());
            //music
            
        }
    }

    private void UpdateEmission(float intensity)
    {
        currentEmissionIntensity = intensity;
        // 设置发光颜色 = 基础颜色 * 强度
        targetMaterial.SetColor("_EmissionColor", emissionColor * Mathf.Pow(2, intensity));
    }

    private IEnumerator AnimateEmission()
    {
        isAnimating = true;

        // 第一阶段：从-10到0，持续1秒
        float startIntensity = currentEmissionIntensity;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float intensity = Mathf.Lerp(startIntensity, 0f, t);
            UpdateEmission(intensity);
            yield return null;
        }

        // 确保精确到达0
        UpdateEmission(0f);

        // 第二阶段：从0到3，持续2秒
        elapsed = 0f;
        duration = 3f;
        startIntensity = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float intensity = Mathf.Lerp(startIntensity, 3f, t);
            UpdateEmission(intensity);
            yield return null;
        }

        // 确保精确到达3
        UpdateEmission(3f);

        isAnimating = false;
        hasCompleted = true; // 标记动画已完成
    }
}