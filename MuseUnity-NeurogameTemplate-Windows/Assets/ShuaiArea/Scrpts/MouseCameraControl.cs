using UnityEngine;
using Cinemachine;

public class MouseCameraControl : MonoBehaviour
{
    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;

    [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    [Tooltip("Sensitivity of mouse rotation")]
    public float RotationSpeed = 1.0f;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private CinemachineVirtualCamera _virtualCamera;

    private void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (_virtualCamera == null)
        {
            Debug.LogError("Virtual Camera component not found!");
            return;
        }

        // 初始化相机角度
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (LockCameraPosition)
        {
            Debug.Log("Camera is locked!");
            return;
        }

        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * RotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * RotationSpeed;

        // 更新相机角度
        _cinemachineTargetYaw += mouseX;
        _cinemachineTargetPitch -= mouseY; // 注意这里是减法，因为向上移动鼠标时Y值是正的

        // 限制相机俯仰角度
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // 应用旋转
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}