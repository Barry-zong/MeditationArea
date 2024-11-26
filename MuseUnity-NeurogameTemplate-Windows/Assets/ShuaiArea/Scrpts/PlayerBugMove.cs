using UnityEngine;
public class PlayerBugMove : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private ArduinoInScene arduinoScript;

    [SerializeField]
    private float moveForce = 10f;
    [SerializeField]
    private float maxVelocity = 10f;

    private const float NEUTRAL_MIN = 500f;
    private const float NEUTRAL_MAX = 510f;
    private const float MAX_SENSOR_VALUE = 1023f;
    private const float MIN_SENSOR_VALUE = 0f;

    private bool useArduinoControl = false;
    private float horizontalMovement = 0f;
    private float verticalMovement = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("未找到Rigidbody组件，请添加Rigidbody组件到物体上。");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (arduinoScript != null)
        {
            float value1 = arduinoScript.arduinoPortNumberOne;
            float value2 = arduinoScript.arduinoPortNumberTwo;
            float value3 = arduinoScript.arduinoPortNumberThree;

            if (value1 != 0 || value2 != 0 || value3 != 0)
            {
                useArduinoControl = true;

                // 前后移动 (Value1) - 对调方向
                if (value1 > NEUTRAL_MAX)
                {
                    // 改为向后移动 (510-1023 映射到 0-(-1))
                    verticalMovement = -Mathf.InverseLerp(NEUTRAL_MAX, MAX_SENSOR_VALUE, value1);
                }
                else if (value1 < NEUTRAL_MIN)
                {
                    // 改为向前移动 (0-500 映射到 0-1)
                    verticalMovement = 1 - Mathf.InverseLerp(MIN_SENSOR_VALUE, NEUTRAL_MIN, value1);
                }
                else
                {
                    verticalMovement = 0f;
                }

                // 左右移动保持不变
                if (value2 > NEUTRAL_MAX)
                {
                    horizontalMovement = -(Mathf.InverseLerp(NEUTRAL_MAX, MAX_SENSOR_VALUE, value2));
                }
                else if (value2 < NEUTRAL_MIN)
                {
                    horizontalMovement = 1 - Mathf.InverseLerp(MIN_SENSOR_VALUE, NEUTRAL_MIN, value2);
                }
                else
                {
                    horizontalMovement = 0f;
                }
            }
            else
            {
                useArduinoControl = false;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 movement;

        if (useArduinoControl)
        {
            movement = new Vector3(horizontalMovement, 0f, verticalMovement) * moveForce;

            // 调试输出最终移动向量
            Debug.Log($"Final Movement: {movement}");
        }
        else
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            movement = new Vector3(horizontalInput, 0f, verticalInput) * moveForce;
        }

        if (movement != Vector3.zero)
        {
            rb.AddForce(movement, ForceMode.Force);

            if (rb.linearVelocity.magnitude > maxVelocity)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
            }
        }
    }
}