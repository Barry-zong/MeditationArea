using UnityEngine;
using LSL;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Linq;
using System.Numerics;
public class MuseToFourWaves : MonoBehaviour
{
    private StreamInlet inlet;
    private float[] sample;
    private const int SAMPLE_SIZE = 256; // ���������Ϊ 256Hz

    void Start()
    {
        var results = LSL.LSL.resolve_stream("type", "EEG");
        if (results.Length > 0)
        {
            inlet = new LSL.StreamInlet(results[0]);
            sample = new float[inlet.info().channel_count()];
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

            // �����Ƶ��ǿ��
            float deltaPower = CalculateFrequencyBandPower(sample, 0.5f, 3f);  // Delta 0.5-3 Hz
            float thetaPower = CalculateFrequencyBandPower(sample, 4f, 7f);    // Theta 4-7 Hz
            float alphaPower = CalculateFrequencyBandPower(sample, 8f, 13f);   // Alpha 8-13 Hz
            float betaPower = CalculateFrequencyBandPower(sample, 14f, 30f);   // Beta 14-30 Hz

            // ��ӡ���
            Debug.Log($"Delta: {deltaPower}, Theta: {thetaPower}, Alpha: {alphaPower}, Beta: {betaPower}");
        }
    }

    // ����ָ��Ƶ�ε�ƽ��ǿ��
    private float CalculateFrequencyBandPower(float[] data, float lowFreq, float highFreq)
    {
        int n = data.Length;
        Complex[] complexData = data.Select(value => new Complex(value, 0)).ToArray();

        // ���������Ϊ 256Hz
        float samplingRate = 256f;
        float freqResolution = samplingRate / n;

        Fourier.Forward(complexData, FourierOptions.Matlab);

        var filtered = complexData
            .Select((value, index) => new { Magnitude = value.Magnitude, Frequency = index * freqResolution })
            .Where(item => item.Frequency >= lowFreq && item.Frequency <= highFreq)
            .Select(item => (float)item.Magnitude)
            .ToArray();

        if (filtered.Length == 0)
        {
            Debug.LogWarning($"û���ҵ� {lowFreq}-{highFreq} Hz ��Χ�ڵ�Ƶ�����ݡ�");
            return 0;
        }

        return filtered.Average();
    }

}
