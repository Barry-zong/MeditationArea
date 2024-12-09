using UnityEngine;

public class FocusLightController : MonoBehaviour
{
    public Material playerMat;
    public Material flowerMat;

    // ��ʼ����ǿ��
    private float playerBaseIntensity = 0f;
    private float flowerBaseIntensity = -2f;
    public float focusValue = 0f;

    private void Start()
    {
        // ��ʼ����Ҳ��ʵķ���ǿ��
        if (playerMat != null)
        {
            playerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, playerBaseIntensity));
        }

        // ��ʼ��������ʵķ���ǿ��
        if (flowerMat != null)
        {
            flowerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, flowerBaseIntensity));
        }
    }

    private void Update()
    {
        // ��ȡ��ǰ��focusֵ
         focusValue = InteraxonInterfacer.Instance.focus;
       

        // ������Ҳ��ʵķ���ǿ�� (ԭʼǿ�� + focus)
        if (playerMat != null)
        {
            float playerIntensity = playerBaseIntensity + focusValue;
            playerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, playerIntensity));
        }

        // ���»�����ʵķ���ǿ�� (ԭʼǿ�� + focus*2)
        if (flowerMat != null)
        {
            float flowerIntensity = flowerBaseIntensity + (focusValue * 2f);
            flowerMat.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, flowerIntensity));
        }
    }

    // �������ʵ��
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