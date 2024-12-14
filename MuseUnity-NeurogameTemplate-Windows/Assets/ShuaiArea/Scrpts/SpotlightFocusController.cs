using UnityEngine;

public class SpotlightFocusController : MonoBehaviour
{
    public Light spotLight; // 引用聚光灯
    public float lightIn1 = 1f; // Focus为0时的强度
    public float lightIn2 = 2f; // Focus为0.4时的强度
    public bool isDebug = false; // 调试模式开关

    [SerializeField]
    [Range(0f, 1f)]
    private float debugFocus = 0f; // 调试用的Focus值

    private float slope; // 斜率
    private float intercept; // 截距

    void Start()
    {
        // 确保已经引用了聚光灯
        if (spotLight == null)
        {
            Debug.LogError("请在Inspector中指定Spot Light!");
            enabled = false;
            return;
        }

        // 计算线性插值的斜率和截距
        slope = (lightIn2 - lightIn1) / 0.4f;
        intercept = lightIn1;
    }

    void Update()
    {
        float currentFocus;

        if (isDebug)
        {
            currentFocus = debugFocus;
        }
        else
        {
            currentFocus = InteraxonInterfacer.Instance.focus;
        }

        // 使用线性插值计算当前强度
        float targetIntensity = CalculateIntensity(currentFocus);
        spotLight.intensity = targetIntensity;

        // Debug日志
        if (isDebug)
        {
            Debug.Log($"Current Focus: {currentFocus}, Light Intensity: {targetIntensity}");
        }
    }

    private float CalculateIntensity(float focus)
    {
        if (focus <= 0f) return lightIn1;
        if (focus >= 0.4f) return lightIn2;

        // 线性插值计算
        return slope * focus + intercept;
    }
}