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
    [SerializeField]
    private float maxGroundAngle = 45f; // 可以行走的最大坡度

    private float oppositeRotation;
    private const float NEUTRAL_MIN = 400f;
    private const float NEUTRAL_MAX = 600f;
    private const float MAX_SENSOR_VALUE = 1023f;
    private const float MIN_SENSOR_VALUE = 0f;
    private bool useArduinoControl = false;
    private float horizontalMovement = 0f;
    private float verticalMovement = 0f;
    private bool canJump = true;
    private float baseRotationY;
    private float targetRotation = 0f;
    private Vector3 groundNormal = Vector3.up; // 存储地面法线

    public float ArduinoJumpControl =0f;

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
        oppositeRotation = baseRotationY + 180f;
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            groundNormal = hit.normal;
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle <= maxGroundAngle;
        }
        groundNormal = Vector3.up;
        return false;
    }

    private void HandleJump()
    {
        if (IsGrounded() && canJump)
        {
            //if (Input.GetKeyDown(KeyCode.Space) )
                if (Input.GetKeyDown(KeyCode.Space)|| ArduinoJumpControl == 1)
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
        if (horizontalInput > 0)
            targetRotation = baseRotationY + 90f;
        else if (horizontalInput < 0)
            targetRotation = baseRotationY - 90f;
        else if (verticalInput < 0)
            targetRotation = baseRotationY + 180f;
        else if (verticalInput > 0)
            targetRotation = baseRotationY;
        else
            targetRotation = oppositeRotation;

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
        HandleArduinoInput();
    }

    private void HandleArduinoInput()
    {
        if (arduinoScript != null)
        {
            float value1 = arduinoScript.arduinoPortNumberOne;
            float value2 = arduinoScript.arduinoPortNumberTwo;
            float value3 = arduinoScript.arduinoPortNumberThree;
            ArduinoJumpControl = value3;
            if (value1 != 0 || value2 != 0)
            {
                useArduinoControl = true;

                if (value1 > NEUTRAL_MAX)
                    verticalMovement = -Mathf.InverseLerp(NEUTRAL_MAX, MAX_SENSOR_VALUE, value1);
                else if (value1 < NEUTRAL_MIN)
                    verticalMovement = 1 - Mathf.InverseLerp(MIN_SENSOR_VALUE, NEUTRAL_MIN, value1);
                else
                    verticalMovement = 0f;

                if (value2 > NEUTRAL_MAX)
                    horizontalMovement = -(Mathf.InverseLerp(NEUTRAL_MAX, MAX_SENSOR_VALUE, value2));
                else if (value2 < NEUTRAL_MIN)
                    horizontalMovement = 1 - Mathf.InverseLerp(MIN_SENSOR_VALUE, NEUTRAL_MIN, value2);
                else
                    horizontalMovement = 0f;

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
            HandleRotation(horizontalInput, verticalInput);
        }

        // 计算移动方向
        Vector3 moveDirection = movement.normalized;

        // 在地面上时，将移动向量投影到地面平面上
        if (IsGrounded())
        {
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
        }

        // 保持当前的Y轴速度（重力影响）
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = currentVelocity.y;

        // 使用力来移动，而不是直接设置速度
        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange.y = 0f; // 不修改垂直方向的速度

        // 应用力
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void OnCollisionStay(Collision collision)
    {
        // 检查碰撞面的角度
        foreach (ContactPoint contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle > maxGroundAngle)
            {
                // 抵消在陡峭表面上的移动力
                Vector3 normalForce = Vector3.Project(rb.linearVelocity, contact.normal);
                rb.AddForce(-normalForce, ForceMode.VelocityChange);
            }
        }
    }
}