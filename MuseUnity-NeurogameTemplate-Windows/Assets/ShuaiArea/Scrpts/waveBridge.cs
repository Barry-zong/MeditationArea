using UnityEngine;
using Interaxon.Libmuse;

public class waveBridge : MonoBehaviour
{
    [Header("目标数值设置")]
    public float targetNumber = 100f;     // 目标数值
    public float MuseNumber = 0f;        // 当前数值
    public float thresholdValueCalm = 10f; // 平静阈值

    [Header("晃动设置")]
    [Tooltip("晃动的最大角度")]
    public Vector3 maxShakeRotation = new Vector3(15f, 15f, 15f);

    [Tooltip("晃动的速度")]
    public float shakeSpeed = 2f;

    [Tooltip("噪声的影响程度")]
    public float noiseStrength = 1f;

    [Header("平滑设置")]
    [Tooltip("回到初始状态的速度")]
    public float returnSpeed = 2f;

    [Tooltip("晃动的平滑度")]
    public float smoothness = 5f;

    // 私有变量
    private Vector3 initialRotation;
    private Vector3 currentNoiseOffset;
    private Vector3 targetRotation;
    private float[] noiseSeeds;

    private void Start()
    {
        // 记录初始旋转
        initialRotation = transform.localEulerAngles;

        // 初始化噪声种子
        noiseSeeds = new float[3];
        for (int i = 0; i < 3; i++)
        {
            noiseSeeds[i] = Random.Range(0f, 1000f);
        }
    }

    private void Update()
    {
        MuseNumber = Mathf.Clamp(InteraxonInterfacer.Instance.focus * 10, 0, 10);

        // 如果当前数值大于目标数值，保持平稳
        if (MuseNumber > targetNumber)
        {
            // 直接平滑过渡到初始状态
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                Quaternion.Euler(initialRotation),
                Time.deltaTime * smoothness
            );
            return; // 直接返回，不执行后续的晃动逻辑
        }

        // 计算差异值 (只在calmNumber <= targetNumber时计算)
        float difference = Mathf.Abs(targetNumber - MuseNumber);

        // 计算晃动强度（0到1之间）
        float shakeFactor = Mathf.Clamp01(difference / thresholdValueCalm);

        if (difference > thresholdValueCalm)
        {
            // 生成基于柏林噪声的随机旋转
            currentNoiseOffset = new Vector3(
                GenerateNoise(0, Time.time * shakeSpeed),
                GenerateNoise(1, Time.time * shakeSpeed),
                GenerateNoise(2, Time.time * shakeSpeed)
            );

            // 应用晃动
            targetRotation = Vector3.Scale(currentNoiseOffset, maxShakeRotation) * shakeFactor;
        }
        else
        {
            // 在阈值内，逐渐回到初始状态
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        }

        // 平滑应用旋转
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            Quaternion.Euler(initialRotation + targetRotation),
            Time.deltaTime * smoothness
        );
    }

    // 生成柏林噪声
    private float GenerateNoise(int index, float time)
    {
        return (Mathf.PerlinNoise(noiseSeeds[index], time) * 2f - 1f) * noiseStrength;
    }

    // 获取当前数值的公共方法
    public float GetCalmNumber()
    {
        return MuseNumber;
    }
}