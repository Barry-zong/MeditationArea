using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float maxGroundAngle = 45f;
    [SerializeField]
    private Camera mainCamera;

    public enum JumpDirection
    {
        WorldUp,      // 世界坐标系的上方向 (0,1,0)
        CharacterUp,  // 角色的局部上方向 (transform.up)
        SurfaceNormal // 根据站立表面的法线方向
    }

    [SerializeField]
    public JumpDirection jumpDirection = JumpDirection.WorldUp;

    private float oppositeRotation;
    public float NEUTRAL_MIN = 320;
    public float NEUTRAL_MAX = 380f;
    public float NEUTRAL_MIN2 = 340;
    public float NEUTRAL_MAX2 = 370f;
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
    public float startTime = 2f;    // 检测时间窗口
    private float currentTime = 0f;  // 当前计时器
    private int jumpCount = 0;       // 跳跃计数器
    private bool wasJumpSignal = false;  // 用于跟踪上一帧的跳跃信号状态
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("未找到Rigidbody组件，请添加Rigidbody组件到物体上。");
            enabled = false;
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;

        baseRotationY = transform.eulerAngles.y;
        oppositeRotation = baseRotationY + 180f;
        ResetJumpCounter();
    }
    private void ResetJumpCounter()
    {
        currentTime = 0f;
        jumpCount = 0;
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
            if (Input.GetKeyDown(KeyCode.Space) || ArduinoJumpControl == 1)
            {
                // 根据选择的跳跃方向设置跳跃力
                Vector3 jumpVector;
                switch (jumpDirection)
                {
                    case JumpDirection.WorldUp:
                        jumpVector = Vector3.up;
                        break;
                    case JumpDirection.CharacterUp:
                        jumpVector = transform.up;
                        break;
                    case JumpDirection.SurfaceNormal:
                        jumpVector = groundNormal;
                        break;
                    default:
                        jumpVector = Vector3.up;
                        break;
                }

                rb.AddForce(jumpVector * jumpForce, ForceMode.Impulse);
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
        jumpTestValue3();
    }

    private void jumpTestValue3()
    {
        // 处理跳跃信号检测
        if (ArduinoJumpControl == 1)
        {
            if (!wasJumpSignal)  // 只在信号从0变为1时计数
            {
                jumpCount++;
                if (jumpCount == 1) // 第一次跳跃时开始计时
                {
                    currentTime = 0f;
                }
            }
            wasJumpSignal = true;
        }
        else
        {
            wasJumpSignal = false;
        }

        // 更新计时器
        if (jumpCount > 0)
        {
            currentTime += Time.deltaTime;

            // 检查是否达到重载条件
            if (jumpCount >= 3)
            {
                Debug.Log("restart the scene");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // 如果超时，重置计数器
            if (currentTime >= startTime)
            {
                ResetJumpCounter();
            }
        }
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

                if (value2 > NEUTRAL_MAX2)
                    horizontalMovement = -(Mathf.InverseLerp(NEUTRAL_MAX, MAX_SENSOR_VALUE, value2));
                else if (value2 < NEUTRAL_MIN2)
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

            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            movement = cameraRight * horizontalInput + cameraForward * verticalInput;

            if (movement.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        Vector3 moveDirection = movement.normalized;

        if (IsGrounded())
        {
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
        }

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = currentVelocity.y;

        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange.y = 0f;

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