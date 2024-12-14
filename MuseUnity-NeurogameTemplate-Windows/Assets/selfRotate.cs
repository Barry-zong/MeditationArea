using UnityEngine;

public class selfRotate : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public bool isdebug = false;
    public float transitionSpeed = 2f; // 控制过渡速度

    private float currentRotationSpeed = 0f; // 当前实际旋转速度

    void Update()
    {
        float targetSpeed = 0f;

        // 判断是否应该旋转
        if (InteraxonInterfacer.Instance.calm > 0.1 || isdebug)
        {
            targetSpeed = rotationSpeed;
        }

        // 平滑过渡到目标速度
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetSpeed, Time.deltaTime * transitionSpeed);

        // 应用旋转
        transform.Rotate(0, currentRotationSpeed * Time.deltaTime, 0);
    }
}