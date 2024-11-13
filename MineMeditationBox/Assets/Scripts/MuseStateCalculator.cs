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
    private const int SAMPLE_SIZE = 256; // 假设采样率为 256Hz

    void Start()
    {
        var results = LSL.LSL.resolve_stream("type", "EEG");
        if (results.Length > 0)
        {
            inlet = new LSL.StreamInlet(results[0]);
            sample = new float[inlet.info().channel_count()];
            Debug.Log("EEG 数据流已连接");
        }
        else
        {
            Debug.LogWarning("未找到 EEG 数据流");
            inlet = null;
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            inlet.pull_sample(sample);

            // 计算特定频段的数值
            float calmValue = CalculateCalmValue(sample);
            float meditationValue = CalculateMeditationValue(sample);
            float focusValue = CalculateFocusValue(sample);

            // 显示各状态数值
            Debug.Log($"平静数值: {calmValue}, 冥想数值: {meditationValue}, 专注数值: {focusValue}");
        }
    }

    // 计算平静数值 (Alpha 和 Theta 的强度)
    private float CalculateCalmValue(float[] data)
    {
        float[] alphaValues = ExtractFrequency(data, 8, 12); // Alpha 8–12 Hz
        float[] thetaValues = ExtractFrequency(data, 4, 8); // Theta 4–8 Hz
        return Average(alphaValues) + Average(thetaValues); // 平静数值
    }

    // 计算冥想数值 (Delta 和 Theta 的强度)
    private float CalculateMeditationValue(float[] data)
    {
        float[] deltaValues = ExtractFrequency(data, 0.5f, 4); // Delta 0.5–4 Hz
        float[] thetaValues = ExtractFrequency(data, 4, 8); // Theta 4–8 Hz
        return Average(deltaValues) + Average(thetaValues); // 冥想数值
    }

    // 计算专注数值 (Beta 强度)
    private float CalculateFocusValue(float[] data)
    {
        float[] betaValues = ExtractFrequency(data, 12, 30); // Beta 12–30 Hz
        return Average(betaValues); // 专注数值
    }

    // 使用 FFT 提取频段数据
    private float[] ExtractFrequency(float[] data, float lowFreq, float highFreq)
    {
        int n = data.Length;
        Complex[] complexData = data.Select(value => new Complex(value, 0)).ToArray();

        // 进行 FFT
        Fourier.Forward(complexData, FourierOptions.Matlab);

        // 计算频率分辨率
        float freqResolution = SAMPLE_SIZE / (float)n;

        // 提取符合频段范围的频率数据
        var filtered = complexData
            .Select((value, index) => new { Magnitude = value.Magnitude, Frequency = index * freqResolution })
            .Where(item => item.Frequency >= lowFreq && item.Frequency <= highFreq)
            .Select(item => (float)item.Magnitude)
            .ToArray();

        return filtered;
    }

    // 计算数组平均值
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
