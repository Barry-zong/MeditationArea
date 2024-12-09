using UnityEngine;

public class RandomRotatingBalls : MonoBehaviour
{
    public bool FinalSceneStart = false;
    public float RadioBall = 5f; // 生成球体的半径范围
    public int BallsNumber = 10; // 要生成的球体数量
    public GameObject RoatateBallPre; // 球体预制体

    // 运动控制参数
    public float minRotationSpeed = 20f;
    public float maxRotationSpeed = 50f;
    public float minOrbitSpeed = 10f;
    public float maxOrbitSpeed = 30f;
    public float minRadius = 1f;
    public float maxRadius = 3f;

    private GameObject[] balls;
    private Vector3[] rotationAxes;
    private float[] rotationSpeeds;
    private float[] orbitSpeeds;
    private float[] orbitRadii;
    private float[] angleOffsets;

    void Start()
    {
        balls = new GameObject[BallsNumber];
        rotationAxes = new Vector3[BallsNumber];
        rotationSpeeds = new float[BallsNumber];
        orbitSpeeds = new float[BallsNumber];
        orbitRadii = new float[BallsNumber];
        angleOffsets = new float[BallsNumber];
    }

    void Update()
    {
        if (FinalSceneStart && balls[0] == null)
        {
            GenerateBalls();
        }

        if (FinalSceneStart && balls[0] != null)
        {
            UpdateBallsMovement();
        }
    }

    void GenerateBalls()
    {
        for (int i = 0; i < BallsNumber; i++)
        {
            // 在球形区域内随机生成位置
            Vector3 randomPosition = Random.insideUnitSphere * RadioBall;
            balls[i] = Instantiate(RoatateBallPre, randomPosition + transform.position, Quaternion.identity);
            balls[i].transform.SetParent(transform);

            // 初始化随机运动参数
            rotationAxes[i] = Random.onUnitSphere;
            rotationSpeeds[i] = Random.Range(minRotationSpeed, maxRotationSpeed);
            orbitSpeeds[i] = Random.Range(minOrbitSpeed, maxOrbitSpeed);
            orbitRadii[i] = Random.Range(minRadius, maxRadius);
            angleOffsets[i] = Random.Range(0f, 360f);
        }
    }

    void UpdateBallsMovement()
    {
        for (int i = 0; i < BallsNumber; i++)
        {
            if (balls[i] != null)
            {
                // 自转运动
                balls[i].transform.Rotate(rotationAxes[i] * rotationSpeeds[i] * Time.deltaTime);

                // 公转运动
                float angle = Time.time * orbitSpeeds[i] + angleOffsets[i];
                Vector3 orbitPosition = new Vector3(
                    Mathf.Cos(angle) * orbitRadii[i],
                    Mathf.Sin(angle * 0.5f) * orbitRadii[i], // 使用不同的角度系数创造不规则运动
                    Mathf.Sin(angle) * orbitRadii[i]
                );
                balls[i].transform.position = transform.position + orbitPosition;
            }
        }
    }

    // 清理生成的球体
    void OnDisable()
    {
        if (balls != null)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i] != null)
                {
                    Destroy(balls[i]);
                }
            }
        }
    }
}