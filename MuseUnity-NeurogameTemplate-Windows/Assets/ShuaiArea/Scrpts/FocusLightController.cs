using UnityEngine;

public class FocusLightController : MonoBehaviour
{
    public Material playerMat;
    public Material flowerMat;
    public Material hideDOOR;

    // 初始发光强度
    private float playerBaseIntensity = 0f;
    private float flowerBaseIntensity = -2f;
    public float focusValue = 0f;

    // 平滑过渡相关参数
    [Range(0.1f, 10f)]
    public float smoothSpeed = 2f; // 过渡速度
    private float currentPlayerIntensity = 0f;
    private float targetPlayerIntensity = 0f;
    private float currentFlowerIntensity = -2f;
    private float targetFlowerIntensity = -2f;

    private void Start()
    {
        // 初始化当前强度值
        currentPlayerIntensity = playerBaseIntensity;
        currentFlowerIntensity = flowerBaseIntensity;

        // 初始化玩家材质的发光强度
        if (playerMat != null)
        {
            playerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, playerBaseIntensity));
        }
        // 初始化花朵材质的发光强度
        if (flowerMat != null)
        {
            flowerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, flowerBaseIntensity));
            hideDOOR.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, flowerBaseIntensity));
        }
    }

    private void Update()
    {
        // 获取当前的focus值
        focusValue = InteraxonInterfacer.Instance.focus;

        // 计算目标强度值
        targetPlayerIntensity = playerBaseIntensity + focusValue * 5f;
        targetFlowerIntensity = flowerBaseIntensity + (focusValue * 25f);

        // 平滑过渡到目标值
        currentPlayerIntensity = Mathf.Lerp(currentPlayerIntensity, targetPlayerIntensity, Time.deltaTime * smoothSpeed);
        currentFlowerIntensity = Mathf.Lerp(currentFlowerIntensity, targetFlowerIntensity, Time.deltaTime * smoothSpeed);

        // 更新玩家材质的发光强度
        if (playerMat != null)
        {
            playerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, currentPlayerIntensity));
        }

        // 更新花朵材质的发光强度
        if (flowerMat != null)
        {
            flowerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, currentFlowerIntensity));
        }
        hideDOOR.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, currentFlowerIntensity));
    }

    private void OnDisable()
    {
        if (playerMat != null && !playerMat.name.Contains("(Instance)"))
        {
            //  Destroy(playerMat);
        }
        if (flowerMat != null && !flowerMat.name.Contains("(Instance)"))
        {
            //  Destroy(flowerMat);
        }
    }
}