using UnityEngine;
using LSL;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Linq;
using System.Numerics;

public class MuseStateCalculator : MonoBehaviour
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
            Debug.LogWarning("δ�ҵ� EEG ������");
            inlet = null;
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            inlet.pull_sample(sample);

            // �����ض�Ƶ�ε���ֵ
            float calmValue = CalculateCalmValue(sample);
            float meditationValue = CalculateMeditationValue(sample);
            float focusValue = CalculateFocusValue(sample);

            // ��ʾ��״̬��ֵ
            Debug.Log($"ƽ����ֵ: {calmValue}, ڤ����ֵ: {meditationValue}, רע��ֵ: {focusValue}");
        }
    }

    // ����ƽ����ֵ (Alpha �� Theta ��ǿ��)
    private float CalculateCalmValue(float[] data)
    {
        float[] alphaValues = ExtractFrequency(data, 8, 12); // Alpha 8�C12 Hz
        float[] thetaValues = ExtractFrequency(data, 4, 8); // Theta 4�C8 Hz
        return Average(alphaValues) + Average(thetaValues); // ƽ����ֵ
    }

    // ����ڤ����ֵ (Delta �� Theta ��ǿ��)
    private float CalculateMeditationValue(float[] data)
    {
        float[] deltaValues = ExtractFrequency(data, 0.5f, 4); // Delta 0.5�C4 Hz
        float[] thetaValues = ExtractFrequency(data, 4, 8); // Theta 4�C8 Hz
        return Average(deltaValues) + Average(thetaValues); // ڤ����ֵ
    }

    // ����רע��ֵ (Beta ǿ��)
    private float CalculateFocusValue(float[] data)
    {
        float[] betaValues = ExtractFrequency(data, 12, 30); // Beta 12�C30 Hz
        return Average(betaValues); // רע��ֵ
    }

    // ʹ�� FFT ��ȡƵ������
    private float[] ExtractFrequency(float[] data, float lowFreq, float highFreq)
    {
        int n = data.Length;
        Complex[] complexData = data.Select(value => new Complex(value, 0)).ToArray();

        // ���� FFT
        Fourier.Forward(complexData, FourierOptions.Matlab);

        // ����Ƶ�ʷֱ���
        float freqResolution = SAMPLE_SIZE / (float)n;

        // ��ȡ����Ƶ�η�Χ��Ƶ������
        var filtered = complexData
            .Select((value, index) => new { Magnitude = value.Magnitude, Frequency = index * freqResolution })
            .Where(item => item.Frequency >= lowFreq && item.Frequency <= highFreq)
            .Select(item => (float)item.Magnitude)
            .ToArray();

        return filtered;
    }

    // ��������ƽ��ֵ
    private float Average(float[] values)
    {
        float sum = 0;
        foreach (float value in values)
        {
            sum += value;
        }
        return values.Length > 0 ? sum / values.Length : 0;
    }
}
