using UnityEngine;
using UnityEngine.Playables;
using Interaxon.Libmuse;

public class FlowerControl : MonoBehaviour
{
    [Header("Components")]
    public PlayableDirector director;
    public GameObject RotateItem;

    [Header("Animation Settings")]
    [Range(0f, 10f)]
    public float flowerMove = 7f;    // 快速动画阈值
    [Range(0f, 10f)]
    public float flowerSlow = 4f;    // 慢速动画阈值
    [Range(0f, 10f)]
    public float flowerStop = 2f;    // 停止动画阈值

    [Header("Rotation Settings")]
    public float rotateSpeed = 90f;   // 基础旋转速度
    public float transitionSpeed = 2f; // 状态过渡速度

    // 私有变量
    private float currentAnimSpeed = 1f;  // 当前动画速度
    private float targetAnimSpeed = 1f;   // 目标动画速度
    private float currentRotateSpeed = 0f;// 当前实际旋转速度

    void Update()
    {
        if (InteraxonInterfacer.Instance.currentConnectionState != ConnectionState.CONNECTED)
        {
            // 如果未连接，缓慢停止所有动作
            targetAnimSpeed = 0f;
            currentRotateSpeed = 0f;
        }
        else
        {
            // 获取并限制flow值
            float MuseNumber_flower = Mathf.Clamp(InteraxonInterfacer.Instance.focus * 10, 0, 10);

            // 根据flow值确定目标动画速度
            if (MuseNumber_flower > flowerMove)
            {
                targetAnimSpeed = 2.0f;  // 加速状态
                currentRotateSpeed = 0f;  // 不旋转
            }
            else if (MuseNumber_flower > flowerSlow)
            {
                targetAnimSpeed = 0.5f;   // 减速状态
                currentRotateSpeed = 0f;  // 不旋转
            }
            else if (MuseNumber_flower > flowerStop)
            {
                targetAnimSpeed = 0f;     // 停止动画
                currentRotateSpeed = rotateSpeed;  // 开始旋转
            }
            else
            {
                targetAnimSpeed = 0f;     // 完全停止
                currentRotateSpeed = 0f;  // 不旋转
            }
        }

        // 平滑过渡动画速度
        currentAnimSpeed = Mathf.Lerp(currentAnimSpeed, targetAnimSpeed, Time.deltaTime * transitionSpeed);
        director.playableGraph.GetRootPlayable(0).SetSpeed(currentAnimSpeed);

        // 应用旋转
        if (RotateItem != null && currentRotateSpeed > 0)
        {
            RotateItem.transform.Rotate(Vector3.up * currentRotateSpeed * Time.deltaTime, Space.Self);
        }
    }

    // 在Unity编辑器中验证设置的阈值
    void OnValidate()
    {
        // 确保阈值的大小关系正确
        flowerMove = Mathf.Max(flowerMove, flowerSlow + 0.1f);
        flowerSlow = Mathf.Max(flowerSlow, flowerStop + 0.1f);
    }
}