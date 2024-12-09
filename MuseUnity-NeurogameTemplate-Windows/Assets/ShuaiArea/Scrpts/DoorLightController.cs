using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class DoorLightController : MonoBehaviour
{
    public Material doorLightMat;
    // 发光强度范围
    public float maxIntensity = 2f;     // Y变化为0时的强度
    public float minIntensity = -5f;    // Y变化为2时的强度
    // Y轴变化量的最大值
    public float maxYChange = 2f;
    private float initialYPosition;

    private void Start()
    {
        initialYPosition = transform.position.y;

        // 确保材质已赋值
        if (doorLightMat == null)
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            doorLightMat = renderer.material;
        }
    }

    private void Update()
    {
        if (doorLightMat == null) return;

        // 计算Y轴变化量的绝对值
        float yChange = Mathf.Abs(transform.position.y - initialYPosition);

        // 将Y轴变化量映射到发光强度范围
        float t = Mathf.Clamp01(yChange / maxYChange);
        float emissionIntensity = Mathf.Lerp(maxIntensity, minIntensity, t);

        // 设置材质的自发光强度
        // 方法1: 如果使用的是材质的_EmissionColor属性
        Color emissionColor = Color.white * Mathf.Pow(2, emissionIntensity);
        doorLightMat.SetColor("_EmissionColor", emissionColor);

        // 方法2: 如果材质有自定义的_EmissionIntensity属性
        doorLightMat.SetFloat("_EmissionIntensity", emissionIntensity);
    }

    // 在脚本禁用或销毁时清理材质实例
    private void OnDisable()
    {
        if (doorLightMat != null && !doorLightMat.name.Contains("(Instance)"))
        {
          //  Destroy(doorLightMat);
        }
    }
}