using UnityEngine;
using System.Collections;

public class MaterialEmissionController : MonoBehaviour
{
    public Material targetMaterial;  // Ŀ�������
    public bool LighttheTree = false;  // ���ƿ���

    private readonly Color emissionColor = new Color(191f / 255f, 166f / 255f, 153f / 255f);  // RGB(191,166,153)
    private bool isAnimating = false;  // ����״̬���
    private bool hasCompleted = false; // ����Ƿ���ɹ���
    private float currentEmissionIntensity = -10f;  // ��ǰ����ǿ��
    public AudioSource audioSource;

    void Start()
    {
        LighttheTree = false;
        hasCompleted = false;
        if (targetMaterial != null)
        {
            // ���ó�ʼ������ɫ��ǿ��
            targetMaterial.EnableKeyword("_EMISSION");
            UpdateEmission(-10f);
        }
    }

    void Update()
    {
        // ֻ����LighttheTreeΪtrue�Ҷ���δ��ʼ��δ���ʱ����������
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
        // ���÷�����ɫ = ������ɫ * ǿ��
        targetMaterial.SetColor("_EmissionColor", emissionColor * Mathf.Pow(2, intensity));
    }

    private IEnumerator AnimateEmission()
    {
        isAnimating = true;

        // ��һ�׶Σ���-10��0������1��
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

        // ȷ����ȷ����0
        UpdateEmission(0f);

        // �ڶ��׶Σ���0��3������2��
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

        // ȷ����ȷ����3
        UpdateEmission(3f);

        isAnimating = false;
        hasCompleted = true; // ��Ƕ��������
    }
}