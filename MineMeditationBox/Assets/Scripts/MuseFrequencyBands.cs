using UnityEngine;
using LSL;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Linq;
using System.Numerics;

public class MuseFrequencyBands : MonoBehaviour
{
    private StreamInlet inlet;
    private float[] sample; // ÿ�ν��յ�����
    private const int SAMPLE_SIZE = 256; // ���������Ϊ 256Hz

    void Start()
    {
        var results = LSL.LSL.resolve_stream("type", "EEG");
        if (results.Length > 0)
        {
            inlet = new LSL.StreamInlet(results[0]);
            sample = new float[inlet.info().channel_count()]; // Ӧ���� 4 ��ͨ��
            Debug.Log("EEG ������������");
        }
        else
        {
            Debug.LogWarning("δ�ҵ� EEG ����������ȷ�� Muse �豸������");
            inlet = null;
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            inlet.pull_sample(sample);

            // �ֱ���ÿ��ͨ��������
            float tp9Delta = CalculateFrequencyBandPower(sample[0], 0.5f, 3f);
            float af7Delta = CalculateFrequencyBandPower(sample[1], 0.5f, 3f);
            float af8Delta = CalculateFrequencyBandPower(sample[2], 0.5f, 3f);
            float tp10Delta = CalculateFrequencyBandPower(sample[3], 0.5f, 3f);

            float tp9Theta = CalculateFrequencyBandPower(sample[0], 4f, 7f);
            float af7Theta = CalculateFrequencyBandPower(sample[1], 4f, 7f);
            float af8Theta = CalculateFrequencyBandPower(sample[2], 4f, 7f);
            float tp10Theta = CalculateFrequencyBandPower(sample[3], 4f, 7f);

            float tp9Alpha = CalculateFrequencyBandPower(sample[0], 8f, 13f);
            float af7Alpha = CalculateFrequencyBandPower(sample[1], 8f, 13f);
            float af8Alpha = CalculateFrequencyBandPower(sample[2], 8f, 13f);
            float tp10Alpha = CalculateFrequencyBandPower(sample[3], 8f, 13f);

            float tp9Beta = CalculateFrequencyBandPower(sample[0], 14f, 30f);
            float af7Beta = CalculateFrequencyBandPower(sample[1], 14f, 30f);
            float af8Beta = CalculateFrequencyBandPower(sample[2], 14f, 30f);
            float tp10Beta = CalculateFrequencyBandPower(sample[3], 14f, 30f);

            // ��ӡ Delta ���εĽ��ʾ��
            Debug.Log($"Delta ���� - TP9: {tp9Delta}, AF7: {af7Delta}, AF8: {af8Delta}, TP10: {tp10Delta}");
            // ��ӡ�������εĽ������֤
        }
    }

    // ����ָ��Ƶ�ε�ƽ��ǿ��
    private float CalculateFrequencyBandPower(float channelData, float lowFreq, float highFreq)
    {
        int n = SAMPLE_SIZE;
        Complex[] complexData = new Complex[n];

        for (int i = 0; i < n; i++)
        {
            complexData[i] = new Complex(channelData, 0);
        }

        // ִ�� FFT
        Fourier.Forward(complexData, FourierOptions.Matlab);

        // ����Ƶ�ʷֱ���
        float freqResolution = 256f / n;

        var filtered = complexData
            .Select((value, index) => new { Magnitude = value.Magnitude, Frequency = index * freqResolution })
            .Where(item => item.Frequency >= lowFreq && item.Frequency <= highFreq)
            .Select(item => (float)item.Magnitude)
            .ToArray();

        return filtered.Length > 0 ? filtered.Average() : 0;
    }
}
