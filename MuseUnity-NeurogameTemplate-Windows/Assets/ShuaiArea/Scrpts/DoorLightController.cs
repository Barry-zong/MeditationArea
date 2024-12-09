using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class DoorLightController : MonoBehaviour
{
    public Material doorLightMat;
    // ����ǿ�ȷ�Χ
    public float maxIntensity = 2f;     // Y�仯Ϊ0ʱ��ǿ��
    public float minIntensity = -5f;    // Y�仯Ϊ2ʱ��ǿ��
    // Y��仯�������ֵ
    public float maxYChange = 2f;
    private float initialYPosition;

    private void Start()
    {
        initialYPosition = transform.position.y;

        // ȷ�������Ѹ�ֵ
        if (doorLightMat == null)
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            doorLightMat = renderer.material;
        }
    }

    private void Update()
    {
        if (doorLightMat == null) return;

        // ����Y��仯���ľ���ֵ
        float yChange = Mathf.Abs(transform.position.y - initialYPosition);

        // ��Y��仯��ӳ�䵽����ǿ�ȷ�Χ
        float t = Mathf.Clamp01(yChange / maxYChange);
        float emissionIntensity = Mathf.Lerp(maxIntensity, minIntensity, t);

        // ���ò��ʵ��Է���ǿ��
        // ����1: ���ʹ�õ��ǲ��ʵ�_EmissionColor����
        Color emissionColor = Color.white * Mathf.Pow(2, emissionIntensity);
        doorLightMat.SetColor("_EmissionColor", emissionColor);

        // ����2: ����������Զ����_EmissionIntensity����
        doorLightMat.SetFloat("_EmissionIntensity", emissionIntensity);
    }

    // �ڽű����û�����ʱ�������ʵ��
    private void OnDisable()
    {
        if (doorLightMat != null && !doorLightMat.name.Contains("(Instance)"))
        {
          //  Destroy(doorLightMat);
        }
    }
}