using UnityEngine;

public class selfRotate : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public bool isdebug = false;
    public float transitionSpeed = 2f; // ���ƹ����ٶ�

    private float currentRotationSpeed = 0f; // ��ǰʵ����ת�ٶ�

    void Update()
    {
        float targetSpeed = 0f;

        // �ж��Ƿ�Ӧ����ת
        if (InteraxonInterfacer.Instance.calm > 0.1 || isdebug)
        {
            targetSpeed = rotationSpeed;
        }

        // ƽ�����ɵ�Ŀ���ٶ�
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetSpeed, Time.deltaTime * transitionSpeed);

        // Ӧ����ת
        transform.Rotate(0, currentRotationSpeed * Time.deltaTime, 0);
    }
}