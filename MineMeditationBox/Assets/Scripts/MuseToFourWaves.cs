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
            Debug.LogWarning("未找到 EEG 数据流，请确保 Muse 设备已连接");
            inlet = null;
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            inlet.pull_sample(sample);

            // 计算各频段强度
            float deltaPower = CalculateFrequencyBandPower(sample, 0.5f, 3f);  // Delta 0.5-3 Hz
            float thetaPower = CalculateFrequencyBandPower(sample, 4f, 7f);    // Theta 4-7 Hz
            float alphaPower = CalculateFrequencyBandPower(sample, 8f, 13f);   // Alpha 8-13 Hz
            float betaPower = CalculateFrequencyBandPower(sample, 14f, 30f);   // Beta 14-30 Hz

            // 打印结果
            Debug.Log($"Delta: {deltaPower}, Theta: {thetaPower}, Alpha: {alphaPower}, Beta: {betaPower}");
        }
    }

    // 计算指定频段的平均强度
    private float CalculateFrequencyBandPower(float[] data, float lowFreq, float highFreq)
    {
        int n = data.Length;
        Complex[] complexData = data.Select(value => new Complex(value, 0)).ToArray();

        // 假设采样率为 256Hz
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
            Debug.LogWarning($"没有找到 {lowFreq}-{highFreq} Hz 范围内的频率数据。");
            return 0;
        }

        return filtered.Average();
    }

}
