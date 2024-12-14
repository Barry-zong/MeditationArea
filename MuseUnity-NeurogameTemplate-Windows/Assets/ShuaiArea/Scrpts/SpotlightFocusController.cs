using UnityEngine;

public class SpotlightFocusController : MonoBehaviour
{
    public Light spotLight; // ���þ۹��
    public float lightIn1 = 1f; // FocusΪ0ʱ��ǿ��
    public float lightIn2 = 2f; // FocusΪ0.4ʱ��ǿ��
    public bool isDebug = false; // ����ģʽ����

    [SerializeField]
    [Range(0f, 1f)]
    private float debugFocus = 0f; // �����õ�Focusֵ

    private float slope; // б��
    private float intercept; // �ؾ�

    void Start()
    {
        // ȷ���Ѿ������˾۹��
        if (spotLight == null)
        {
            Debug.LogError("����Inspector��ָ��Spot Light!");
            enabled = false;
            return;
        }

        // �������Բ�ֵ��б�ʺͽؾ�
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

        // ʹ�����Բ�ֵ���㵱ǰǿ��
        float targetIntensity = CalculateIntensity(currentFocus);
        spotLight.intensity = targetIntensity;

        // Debug��־
        if (isDebug)
        {
            Debug.Log($"Current Focus: {currentFocus}, Light Intensity: {targetIntensity}");
        }
    }

    private float CalculateIntensity(float focus)
    {
        if (focus <= 0f) return lightIn1;
        if (focus >= 0.4f) return lightIn2;

        // ���Բ�ֵ����
        return slope * focus + intercept;
    }
}