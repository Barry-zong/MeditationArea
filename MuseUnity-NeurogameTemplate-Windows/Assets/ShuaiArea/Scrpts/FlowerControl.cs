using UnityEditor;
using UnityEngine;

public class FlowerControl : MonoBehaviour
{
    public float minThreshold = 0.3f;
    public float maxThreshold = 0.7f;
    public float baseSpeed = 1f;
    public float speedMultiplier = 2f;
    public float Musedate = 0f;

    private Animator animator;
    private float currentSpeed = 0f;
    private string clipName = "warratahtest_02_Time"; // 你的动画片段名称

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("找不到 Animator 组件");
            return;
        }

        // 获取动画片段并设置循环
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                SerializedObject serializedClip = new SerializedObject(clip);
                SerializedProperty loopTime = serializedClip.FindProperty("m_LoopTime");
                loopTime.boolValue = true;
                serializedClip.ApplyModifiedProperties();
                break;
            }
        }

        // 初始设置
        animator.speed = 0;
    }

    void Update()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        if (Musedate < minThreshold)
        {
            currentSpeed = 0f;
            animator.speed = 0;
            // 重置到开始
            animator.Play(clipName, 0, 0f);
        }
        else if (Musedate >= minThreshold && Musedate < maxThreshold)
        {
            currentSpeed = baseSpeed;
            animator.speed = currentSpeed;
        }
        else if (Musedate >= maxThreshold)
        {
            float speedIncrease = (Musedate - maxThreshold) * speedMultiplier;
            currentSpeed = baseSpeed + speedIncrease;
            animator.speed = currentSpeed;
        }

        // 手动循环控制
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f && Musedate >= minThreshold)
        {
            animator.Play(clipName, 0, 0f);
        }
    }

    public void SetMusedate(float value)
    {
        Musedate = value;
        Debug.Log($"当前 Musedate: {value}, 速度: {currentSpeed}");
    }
}