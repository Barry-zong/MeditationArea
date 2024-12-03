using UnityEngine;

public class PlayerBugMove : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private ArduinoInScene arduinoScript;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float groundCheckDistance = 0.1f;
    [SerializeField]
    private float rotationSpeed = 720f;
    [SerializeField]
    private LayerMask groundLayer;

    private const float NEUTRAL_MIN = 450f;
    private const float NEUTRAL_MAX = 520f;
    private const float MAX_SENSOR_VALUE = 1023f;
    private const float MIN_SENSOR_VALUE = 0f;
    private bool useArduinoControl = false;
    private float horizontalMovement = 0f;
    private float verticalMovement = 0f;
    private bool canJump = true;
    private float baseRotationY;
    private float targetRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("未找到Rigidbody组件，请添加Rigidbody组件到物体上。");
            enabled = false;
            return;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;

        baseRotationY = transform.eulerAngles.y;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void HandleJump()
    {
        if (IsGrounded() && canJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                canJump = false;
                Invoke("ResetJump", 0.1f);
            }
        }
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void HandleRotation(float horizontalInput, float verticalInput)
    {
        // 处理水平移动的旋转
        if (horizontalInput > 0) // 向右
        {
            targetRotation = baseRotationY + 90f;
        }
        else if (horizontalInput < 0) // 向左
        {
            targetRotation = baseRotationY - 90f;
        }
        else if (verticalInput < 0) // 向后
        {
            targetRotation = baseRotationY + 180f;
        }
        else // 向前或静止
        {
            targetRotation = baseRotationY;
        }

        Quaternion targetQuaternion = Quaternion.Euler(
            transform.eulerAngles.x,
            targetRotation,
            transform.eulerAngles.z
        );

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetQuaternion,
            rotationSpeed * Time.deltaTime
        );
    }


    private void Update()
    {
        HandleJump();

        if (arduinoScript != null)
        {
            float value1 = arduinoScript.arduinoPortNumberOne;
            float value2 = arduinoScript.arduinoPortNumberTwo;

            if (value1 != 0 || value2 != 0)
            {
                useArduinoControl = true;

                if (value1 > NEUTRAL_MAX)
                {
                    verticalMovement = -Mathf.InverseLerp(NEUTRAL_MAX, MAX_SENSOR_VALUE, value1);
                }
                else if (value1 < NEUTRAL_MIN)
                {
                    verticalMovement = 1 - Mathf.InverseLerp(MIN_SENSOR_VALUE, NEUTRAL_MIN, value1);
                }
                else
                {
                    verticalMovement = 0f;
                }

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

                // 传入垂直和水平移动值
                HandleRotation(horizontalMovement, verticalMovement);
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
            movement = new Vector3(horizontalMovement, 0f, verticalMovement);
        }
        else
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            movement = new Vector3(horizontalInput, 0f, verticalInput);

            // 传入垂直和水平输入值
            HandleRotation(horizontalInput, verticalInput);
        }

        float currentYVelocity = rb.linearVelocity.y;
        Vector3 newVelocity = movement * moveSpeed;
        newVelocity.y = currentYVelocity;
        rb.linearVelocity = newVelocity;
    }
}