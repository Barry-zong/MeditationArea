using UnityEngine;

public class FocusLightController : MonoBehaviour
{
    public Material playerMat;
    public Material flowerMat;
    public Material hideDOOR;

    // ��ʼ����ǿ��
    private float playerBaseIntensity = 0f;
    private float flowerBaseIntensity = -2f;
    public float focusValue = 0f;

    // ƽ��������ز���
    [Range(0.1f, 10f)]
    public float smoothSpeed = 2f; // �����ٶ�
    private float currentPlayerIntensity = 0f;
    private float targetPlayerIntensity = 0f;
    private float currentFlowerIntensity = -2f;
    private float targetFlowerIntensity = -2f;

    private void Start()
    {
        // ��ʼ����ǰǿ��ֵ
        currentPlayerIntensity = playerBaseIntensity;
        currentFlowerIntensity = flowerBaseIntensity;

        // ��ʼ����Ҳ��ʵķ���ǿ��
        if (playerMat != null)
        {
            playerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, playerBaseIntensity));
        }
        // ��ʼ��������ʵķ���ǿ��
        if (flowerMat != null)
        {
            flowerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, flowerBaseIntensity));
            hideDOOR.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, flowerBaseIntensity));
        }
    }

    private void Update()
    {
        // ��ȡ��ǰ��focusֵ
        focusValue = InteraxonInterfacer.Instance.focus;

        // ����Ŀ��ǿ��ֵ
        targetPlayerIntensity = playerBaseIntensity + focusValue * 5f;
        targetFlowerIntensity = flowerBaseIntensity + (focusValue * 25f);

        // ƽ�����ɵ�Ŀ��ֵ
        currentPlayerIntensity = Mathf.Lerp(currentPlayerIntensity, targetPlayerIntensity, Time.deltaTime * smoothSpeed);
        currentFlowerIntensity = Mathf.Lerp(currentFlowerIntensity, targetFlowerIntensity, Time.deltaTime * smoothSpeed);

        // ������Ҳ��ʵķ���ǿ��
        if (playerMat != null)
        {
            playerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, currentPlayerIntensity));
        }

        // ���»�����ʵķ���ǿ��
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