using UnityEngine;

public class VerticleObjectFloating : MonoBehaviour
{
    [Tooltip("控制浮动的幅度和速度")]
    public float A = 1.0f;

    [Tooltip("作为随机种子，影响浮动的随机性")]
    public float B = 0.0f;

    [Tooltip("平滑过渡的速度，值越小过渡越平滑")]
    public float smoothSpeed = 2.0f;

    private float initialY;
    private float randomOffset;
    private float currentA;  // 用于平滑过渡的当前A值
    private float targetA;   // 目标A值

    // 在Start中初始化
    void Start()
    {
        // 记录初始Y位置
        initialY = transform.position.y;

        // 使用B作为随机种子生成一个偏移值
        Random.InitState((int)(B * 1000));
        randomOffset = Random.Range(0f, 2f * Mathf.PI);

        // 初始化当前A值
        currentA = A;
        targetA = A;
    }

    // 在每一帧中更新位置
    void Update()
    {
        // 更新目标A值
        targetA = InteraxonInterfacer.Instance.flow / 2;

        // 平滑过渡到目标A值
        currentA = Mathf.Lerp(currentA, targetA, Time.deltaTime * smoothSpeed);

        // 计算当前的Y偏移
        float amplitude = currentA;             // 振幅使用平滑后的A
        float frequency = currentA * 0.5f;      // 频率也使用平滑后的A

        // 使用正弦函数创建浮动效果，加入随机偏移使不同物体有不同的浮动节奏
        float yOffset = amplitude * Mathf.Sin((Time.time * frequency) + randomOffset);

        // 应用偏移量到物体位置（使用SmoothDamp让位置变化也更平滑）
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(currentPos.x, initialY + yOffset, currentPos.z);
        Vector3 velocity = Vector3.zero;

        transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref velocity, 0.1f);
    }
}