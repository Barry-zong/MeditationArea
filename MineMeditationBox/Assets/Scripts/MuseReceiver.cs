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
        // 查找类型为 "EEG" 的数据流
        var results = LSL.LSL.resolve_stream("type", "EEG");
        if (results.Length > 0)
        {
            inlet = new LSL.StreamInlet(results[0]);
            sample = new float[inlet.info().channel_count()];
        }
        else
        {
            Debug.LogError("未找到 EEG 数据流。");
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            // 从流中获取样本
            inlet.pull_sample(sample);
            Debug.Log("接收到的 EEG 数据：" + sample[0]);  // 显示第一个通道的数据
        }
    }
    */
    void Start()
    {
        // 查找类型为 "EEG" 的数据流
        var results = LSL.LSL.resolve_stream("type", "EEG");
        if (results.Length > 0)
        {
            inlet = new LSL.StreamInlet(results[0]);
            sample = new float[inlet.info().channel_count()];
        }
        else
        {
            Debug.LogError("未找到 EEG 数据流。");
        }
    }

    void Update()
    {
        if (inlet != null)
        {
            inlet.pull_sample(sample);

            // 显示不同通道的脑波数据
            float af7 = sample[0]; // AF7 通道
            float af8 = sample[1]; // AF8 通道
            float tp9 = sample[2]; // TP9 通道
            float tp10 = sample[3]; // TP10 通道

            // 在控制台显示数据
           // Debug.Log($"AF7: {af7}, AF8: {af8}, TP9: {tp9}, TP10: {tp10}");
            EegData = $"AF7: {af7}, AF8: {af8}, TP9: {tp9}, TP10: {tp10}";

            // 您可以在此处进一步解析特定频段的信号
            // DisplayBrainWaves(af7, af8, tp9, tp10);
        }
    }

    // 示例：将脑波数据转化为 Alpha、Beta、Theta、Delta 等频段的伪代码
    private void DisplayBrainWaves(float af7, float af8, float tp9, float tp10)
    {
        // 例如：提取 Alpha 波段
        float alphaAF7 = ProcessAlphaWave(af7);
        float alphaAF8 = ProcessAlphaWave(af8);

        // 显示 Alpha 波段数据
        Debug.Log($"Alpha AF7: {alphaAF7}, Alpha AF8: {alphaAF8}");
    }

    // 假设一个函数可以处理 Alpha 波段数据的伪代码
    private float ProcessAlphaWave(float channelData)
    {
        // 在这里进行数据处理，将原始数据分解为特定频段
        // 具体公式需要根据 Muse 数据文档实现
        return channelData; // 这里只是示例
    }
}
