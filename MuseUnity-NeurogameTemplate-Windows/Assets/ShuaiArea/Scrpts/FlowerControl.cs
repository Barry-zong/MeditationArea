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
    private string clipName = "warratahtest_02_Time"; // ��Ķ���Ƭ������

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("�Ҳ��� Animator ���");
            return;
        }

        // ��ȡ����Ƭ�β�����ѭ��
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

        // ��ʼ����
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
            // ���õ���ʼ
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

        // �ֶ�ѭ������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f && Musedate >= minThreshold)
        {
            animator.Play(clipName, 0, 0f);
        }
    }

    public void SetMusedate(float value)
    {
        Musedate = value;
        Debug.Log($"��ǰ Musedate: {value}, �ٶ�: {currentSpeed}");
    }
}