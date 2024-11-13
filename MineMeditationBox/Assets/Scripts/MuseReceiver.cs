using UnityEngine;
using LSL;



public class MuseReceiver : MonoBehaviour
{
    private StreamInlet inlet;
    private float[] sample;
    public string EegData;

    /*
    void Start()
    {
        // ��������Ϊ "EEG" ��������
        var results = LSL.LSL.resolve_stream("type", "EEG");
        if (results.Length > 0)
        {
            inlet = new LSL.StreamInlet(results[0]);
            sample = new float[inlet.info().channel_count()];
        }
        else
        {
            Debug.LogError("δ�ҵ� EEG ��������");
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            // �����л�ȡ����
            inlet.pull_sample(sample);
            Debug.Log("���յ��� EEG ���ݣ�" + sample[0]);  // ��ʾ��һ��ͨ��������
        }
    }
    */
    void Start()
    {
        // ��������Ϊ "EEG" ��������
        var results = LSL.LSL.resolve_stream("type", "EEG");
        if (results.Length > 0)
        {
            inlet = new LSL.StreamInlet(results[0]);
            sample = new float[inlet.info().channel_count()];
        }
        else
        {
            Debug.LogError("δ�ҵ� EEG ��������");
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            inlet.pull_sample(sample);

            // ��ʾ��ͬͨ�����Բ�����
            float af7 = sample[0]; // AF7 ͨ��
            float af8 = sample[1]; // AF8 ͨ��
            float tp9 = sample[2]; // TP9 ͨ��
            float tp10 = sample[3]; // TP10 ͨ��

            // �ڿ���̨��ʾ����
           // Debug.Log($"AF7: {af7}, AF8: {af8}, TP9: {tp9}, TP10: {tp10}");
            EegData = $"AF7: {af7}, AF8: {af8}, TP9: {tp9}, TP10: {tp10}";

            // �������ڴ˴���һ�������ض�Ƶ�ε��ź�
            // DisplayBrainWaves(af7, af8, tp9, tp10);
        }
    }

    // ʾ�������Բ�����ת��Ϊ Alpha��Beta��Theta��Delta ��Ƶ�ε�α����
    private void DisplayBrainWaves(float af7, float af8, float tp9, float tp10)
    {
        // ���磺��ȡ Alpha ����
        float alphaAF7 = ProcessAlphaWave(af7);
        float alphaAF8 = ProcessAlphaWave(af8);

        // ��ʾ Alpha ��������
        Debug.Log($"Alpha AF7: {alphaAF7}, Alpha AF8: {alphaAF8}");
    }

    // ����һ���������Դ��� Alpha �������ݵ�α����
    private float ProcessAlphaWave(float channelData)
    {
        // ������������ݴ�����ԭʼ���ݷֽ�Ϊ�ض�Ƶ��
        // ���幫ʽ��Ҫ���� Muse �����ĵ�ʵ��
        return channelData; // ����ֻ��ʾ��
    }
}
