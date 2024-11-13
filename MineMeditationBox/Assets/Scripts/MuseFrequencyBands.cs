using UnityEngine;
using LSL;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Linq;
using System.Numerics;

public class MuseFrequencyBands : MonoBehaviour
{
    private StreamInlet inlet;
    private float[] sample; // 每次接收的样本
    private const int SAMPLE_SIZE = 256; // 假设采样率为 256Hz

    void Start()
    {
        var results = LSL.LSL.resolve_stream("type", "EEG");
        if (results.Length > 0)
        {
            inlet = new LSL.StreamInlet(results[0]);
            sample = new float[inlet.info().channel_count()]; // 应该有 4 个通道
            Debug.Log("EEG 数据流已连接");
        }
        else
        {
            Debug.LogWarning("未找到 EEG 数据流，请确保 Muse 设备已连接");
            inlet = null;
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            inlet.pull_sample(sample);

            // 分别处理每个通道的数据
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

            // 打印 Delta 波段的结果示例
            Debug.Log($"Delta 波段 - TP9: {tp9Delta}, AF7: {af7Delta}, AF8: {af8Delta}, TP10: {tp10Delta}");
            // 打印其他波段的结果以验证
        }
    }

    // 计算指定频段的平均强度
    private float CalculateFrequencyBandPower(float channelData, float lowFreq, float highFreq)
    {
        int n = SAMPLE_SIZE;
        Complex[] complexData = new Complex[n];

        for (int i = 0; i < n; i++)
        {
            complexData[i] = new Complex(channelData, 0);
        }

        // 执行 FFT
        Fourier.Forward(complexData, FourierOptions.Matlab);

        // 计算频率分辨率
        float freqResolution = 256f / n;

        var filtered = complexData
            .Select((value, index) => new { Magnitude = value.Magnitude, Frequency = index * freqResolution })
            .Where(item => item.Frequency >= lowFreq && item.Frequency <= highFreq)
            .Select(item => (float)item.Magnitude)
            .ToArray();

        return filtered.Length > 0 ? filtered.Average() : 0;
    }
}
