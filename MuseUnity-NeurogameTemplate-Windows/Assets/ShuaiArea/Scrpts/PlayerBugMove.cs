using UnityEngine;

public class PlayerBugMove : MonoBehaviour
{
    private Rigidbody rb;

    // �ƶ����Ĵ�С
    [SerializeField]
    private float moveForce = 10f;

    // ����ٶ�����
    [SerializeField]
    private float maxVelocity = 10f;

    void Start()
    {
        // ��ȡRigidbody���
        rb = GetComponent<Rigidbody>();

        // ȷ����Rigidbody���
        if (rb == null)
        {
            Debug.LogError("δ�ҵ�Rigidbody�����������Rigidbody����������ϡ�");
            enabled = false;
            return;
        }
    }

    void FixedUpdate()
    {
        // ��ȡˮƽ�ʹ�ֱ����
        float horizontalInput = Input.GetAxis("Horizontal"); // A��D
        float verticalInput = Input.GetAxis("Vertical");     // W��S

        // �����ƶ���������
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        // ����������ʩ����
        if (movement != Vector3.zero)
        {
            // ʩ�������ƶ�����
            rb.AddForce(movement * moveForce, ForceMode.Force);

            // ��������ٶ�
            if (rb.linearVelocity.magnitude > maxVelocity)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
            }
        }
    }
}
