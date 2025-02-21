using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    [Header("跳跃设置")]
    public float jumpForce = 10f;        // 跳跃力度
    public float cooldownTime = 5f;      // 冷却时间（秒）
    public float maxSlopeAngle = 60f;    // 最大坡度角度

    [Header("状态")]
    public bool canJump = true;          // 是否可以跳跃
    public bool isInCooldown = false;    // 是否在冷却中

    private Rigidbody rb;                // 刚体组件
    private float cooldownTimer = 0f;    // 冷却计时器

    void Start()
    {
        // 获取刚体组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("物体需要有Rigidbody组件!");
        }
    }

    void Update()
    {
        // 检测跳跃输入
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isInCooldown)
        {
            Jump();
        }

        // 处理冷却计时
        if (isInCooldown)
        {
            cooldownTimer += Time.deltaTime;

            // 如果冷却时间到，结束冷却
            if (cooldownTimer >= cooldownTime)
            {
                EndCooldown();
            }
        }
    }

    // 跳跃方法
    void Jump()
    {
        // 向上施加力
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // 开始冷却
        StartCooldown();
    }

    // 开始冷却
    void StartCooldown()
    {
        isInCooldown = true;
        canJump = false;
        cooldownTimer = 0f;

     //   Debug.Log("跳跃冷却开始");
    }

    // 结束冷却
    void EndCooldown()
    {
        isInCooldown = false;
        canJump = true;
        cooldownTimer = 0f;

     //   Debug.Log("跳跃冷却结束");
    }

    // 碰撞检测
    void OnCollisionEnter(Collision collision)
    {
        // 只有在冷却状态才检查碰撞
        if (isInCooldown)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // 计算接触点的法线与上方向的夹角
                float slopeAngle = Vector3.Angle(contact.normal, Vector3.up);

                // 如果坡度小于设定的最大值，则结束冷却
                if (slopeAngle < maxSlopeAngle)
                {
                  //  Debug.Log("检测到坡度为 " + slopeAngle + " 度的表面，结束冷却");
                    EndCooldown();
                    break;
                }
                else
                {
                    //Debug.Log("检测到坡度为 " + slopeAngle + " 度的表面，坡度过大，不结束冷却");
                }
            }
        }
    }
}