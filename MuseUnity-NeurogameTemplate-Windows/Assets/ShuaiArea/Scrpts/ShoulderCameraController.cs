using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Orbit Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float rotationSmoothSpeed = 15f;

    [Header("Target Following")]
    [SerializeField] private float verticalSmoothTime = 0.15f;
    [SerializeField] private float maxVerticalSpeed = 10f;
    [SerializeField] private float verticalDampingDistance = 0.5f;

    [Header("FOV Settings")]
    [SerializeField] private float defaultFOV = 60f;
    [SerializeField] private float minFOV = 30f;
    [SerializeField] private float maxFOV = 90f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float fovSmoothTime = 0.1f;  // FOV平滑时间

    [Header("Angle Limits")]
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 80f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;

    // 原有变量
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private Quaternion targetRotation;
    private float dampedTargetHeight;
    private float verticalVelocity;
    private Vector3 lastTargetPosition;

    // FOV相关变量
    private Camera mainCamera;
    private float targetFOV;
    private float fovVelocity;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("请设置相机跟随目标！");
            enabled = false;
            return;
        }

        // 初始化相机组件和FOV
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("找不到Camera组件！");
            enabled = false;
            return;
        }

        // 初始化FOV
        mainCamera.fieldOfView = defaultFOV;
        targetFOV = defaultFOV;

        // 初始化其他参数
        currentRotationY = transform.eulerAngles.y;
        currentRotationX = transform.eulerAngles.x;
        dampedTargetHeight = target.position.y + targetOffset.y;
        lastTargetPosition = target.position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = HandleTargetFollowing();
        HandleRotation(targetPosition);
        HandleZoom();
        UpdateCameraPosition(targetPosition);
    }

    private void HandleZoom()
    {
        // 处理鼠标滚轮输入
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // 按住Ctrl时滚轮控制FOV
            targetFOV = Mathf.Clamp(targetFOV - scrollDelta * zoomSpeed * 10f, minFOV, maxFOV);

            // 平滑过渡FOV
            mainCamera.fieldOfView = Mathf.SmoothDamp(
                mainCamera.fieldOfView,
                targetFOV,
                ref fovVelocity,
                fovSmoothTime
            );
        }
        else
        {
            // 不按Ctrl时滚轮控制距离
            distance = Mathf.Clamp(distance - scrollDelta * 5f, minDistance, maxDistance);
        }
    }

    private Vector3 HandleTargetFollowing()
    {
        float targetHeight = target.position.y + targetOffset.y;
        float heightDiff = targetHeight - dampedTargetHeight;

        float dampingMultiplier = Mathf.Clamp01(Mathf.Abs(heightDiff) / verticalDampingDistance);
        float currentSmoothTime = verticalSmoothTime * (1f + dampingMultiplier);

        dampedTargetHeight = Mathf.SmoothDamp(
            dampedTargetHeight,
            targetHeight,
            ref verticalVelocity,
            currentSmoothTime,
            maxVerticalSpeed
        );

        return new Vector3(
            target.position.x + targetOffset.x,
            dampedTargetHeight,
            target.position.z + targetOffset.z
        );
    }

    private void HandleRotation(Vector3 targetPosition)
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentRotationY += mouseX;
        currentRotationX -= mouseY;
        currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);

        targetRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
    }

    private void UpdateCameraPosition(Vector3 targetPosition)
    {
        Vector3 desiredPosition = targetPosition - (targetRotation * Vector3.forward * distance);
        transform.position = desiredPosition;
        transform.rotation = targetRotation;
        transform.LookAt(targetPosition);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 公共方法
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            dampedTargetHeight = target.position.y + targetOffset.y;
            lastTargetPosition = target.position;
        }
    }

    public void ResetFOV()
    {
        targetFOV = defaultFOV;
    }

    public void SetFOV(float newFOV)
    {
        targetFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);
    }
}