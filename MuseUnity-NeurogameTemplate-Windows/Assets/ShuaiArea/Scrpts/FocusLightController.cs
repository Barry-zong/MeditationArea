using UnityEngine;

public class FocusLightController : MonoBehaviour
{
    public Material playerMat;
    public Material flowerMat;

    // 初始发光强度
    private float playerBaseIntensity = 0f;
    private float flowerBaseIntensity = -2f;
    public float focusValue = 0f;

    private void Start()
    {
        // 初始化玩家材质的发光强度
        if (playerMat != null)
        {
            playerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, playerBaseIntensity));
        }

        // 初始化花朵材质的发光强度
        if (flowerMat != null)
        {
            flowerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, flowerBaseIntensity));
        }
    }

    private void Update()
    {
        // 获取当前的focus值
         focusValue = InteraxonInterfacer.Instance.focus;
       

        // 更新玩家材质的发光强度 (原始强度 + focus)
        if (playerMat != null)
        {
            float playerIntensity = playerBaseIntensity + focusValue;
            playerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, playerIntensity));
        }

        // 更新花朵材质的发光强度 (原始强度 + focus*2)
        if (flowerMat != null)
        {
            float flowerIntensity = flowerBaseIntensity + (focusValue * 2f);
            flowerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, flowerIntensity));
        }
    }

    // 清理材质实例
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