using UnityEngine;
using Interaxon.Libmuse;

public class waveBridge : MonoBehaviour
{
    [Header("Ŀ����ֵ����")]
    public float targetNumber = 100f;     // Ŀ����ֵ
    public float MuseNumber = 0f;        // ��ǰ��ֵ
    public float thresholdValueCalm = 10f; // ƽ����ֵ

    [Header("�ζ�����")]
    [Tooltip("�ζ������Ƕ�")]
    public Vector3 maxShakeRotation = new Vector3(15f, 15f, 15f);

    [Tooltip("�ζ����ٶ�")]
    public float shakeSpeed = 2f;

    [Tooltip("������Ӱ��̶�")]
    public float noiseStrength = 1f;

    [Header("ƽ������")]
    [Tooltip("�ص���ʼ״̬���ٶ�")]
    public float returnSpeed = 2f;

    [Tooltip("�ζ���ƽ����")]
    public float smoothness = 5f;

    // ˽�б���
    private Vector3 initialRotation;
    private Vector3 currentNoiseOffset;
    private Vector3 targetRotation;
    private float[] noiseSeeds;

    private void Start()
    {
        // ��¼��ʼ��ת
        initialRotation = transform.localEulerAngles;

        // ��ʼ����������
        noiseSeeds = new float[3];
        for (int i = 0; i < 3; i++)
        {
            noiseSeeds[i] = Random.Range(0f, 1000f);
        }
    }

    private void Update()
    {
        MuseNumber = Mathf.Clamp(InteraxonInterfacer.Instance.focus * 10, 0, 10);

        // �����ǰ��ֵ����Ŀ����ֵ������ƽ��
        if (MuseNumber > targetNumber)
        {
            // ֱ��ƽ�����ɵ���ʼ״̬
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                Quaternion.Euler(initialRotation),
                Time.deltaTime * smoothness
            );
            return; // ֱ�ӷ��أ���ִ�к����Ļζ��߼�
        }

        // �������ֵ (ֻ��calmNumber <= targetNumberʱ����)
        float difference = Mathf.Abs(targetNumber - MuseNumber);

        // ����ζ�ǿ�ȣ�0��1֮�䣩
        float shakeFactor = Mathf.Clamp01(difference / thresholdValueCalm);

        if (difference > thresholdValueCalm)
        {
            // ���ɻ��ڰ��������������ת
            currentNoiseOffset = new Vector3(
                GenerateNoise(0, Time.time * shakeSpeed),
                GenerateNoise(1, Time.time * shakeSpeed),
                GenerateNoise(2, Time.time * shakeSpeed)
            );

            // Ӧ�ûζ�
            targetRotation = Vector3.Scale(currentNoiseOffset, maxShakeRotation) * shakeFactor;
        }
        else
        {
            // ����ֵ�ڣ��𽥻ص���ʼ״̬
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        }

        // ƽ��Ӧ����ת
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            Quaternion.Euler(initialRotation + targetRotation),
            Time.deltaTime * smoothness
        );
    }

    // ���ɰ�������
    private float GenerateNoise(int index, float time)
    {
        return (Mathf.PerlinNoise(noiseSeeds[index], time) * 2f - 1f) * noiseStrength;
    }

    // ��ȡ��ǰ��ֵ�Ĺ�������
    public float GetCalmNumber()
    {
        return MuseNumber;
    }
}