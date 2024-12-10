using UnityEngine;

public class RandomRotatingBalls : MonoBehaviour
{
    public bool FinalSceneStart = false;
    public float RadioBall = 5f;
    public int BallsNumber = 10;
    public GameObject RoatateBallPre;

    // 运动控制参数
    public float minRotationSpeed = 20f;
    public float maxRotationSpeed = 50f;
    public float minOrbitSpeed = 10f;
    public float maxOrbitSpeed = 30f;
    public float minRadius = 1f;
    public float maxRadius = 3f;

    // 添加速度插值控制
    [Range(0f, 1f)]
    public float speedMultiplier = 1f;  // 速度系数，范围0-1
    public float FocusValue = 0f;
    public Material rotateBalls;

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
        rotateBalls.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, InteraxonInterfacer.Instance.calm*6));
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
            Vector3 randomPosition = Random.insideUnitSphere * RadioBall;
            balls[i] = Instantiate(RoatateBallPre, randomPosition + transform.position, Quaternion.identity);
            balls[i].transform.SetParent(transform);

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
                // 使用speedMultiplier来影响自转速度
                float currentRotationSpeed = rotationSpeeds[i] * speedMultiplier + InteraxonInterfacer.Instance.focus*FocusValue;
                balls[i].transform.Rotate(rotationAxes[i] * currentRotationSpeed * Time.deltaTime);

                // 使用speedMultiplier来影响公转速度
                float currentOrbitSpeed = orbitSpeeds[i] * speedMultiplier;
                float angle = Time.time * currentOrbitSpeed + angleOffsets[i];

                Vector3 orbitPosition = new Vector3(
                    Mathf.Cos(angle) * orbitRadii[i],
                    Mathf.Sin(angle * 0.5f) * orbitRadii[i],
                    Mathf.Sin(angle) * orbitRadii[i]
                );

                balls[i].transform.position = transform.position + orbitPosition;
            }
        }
    }

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