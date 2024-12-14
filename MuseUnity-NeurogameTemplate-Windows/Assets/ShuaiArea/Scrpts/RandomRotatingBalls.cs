using UnityEngine;

public class RandomRotatingBalls : MonoBehaviour
{
    [Header("Scene Control")]
    public bool FinalSceneStart = false;
    public float fcf = 1;

    [Header("Ball Generation Settings")]
    [Tooltip("Prefab of the ball to be instantiated")]
    public GameObject RoatateBallPre;
    [Tooltip("Number of balls to generate")]
    [Range(1, 100)]
    public int BallsNumber = 10;
    [Tooltip("Material for the rotating balls")]
    public Material rotateBalls;

    [Header("Distribution Settings")]
    [Tooltip("Initial spawn radius from the center")]
    [Range(1f, 20f)]
    public float RadioBall = 5f;
    [Tooltip("Minimum distance between balls")]
    [Range(0.1f, 5f)]
    public float MinBallDistance = 0.5f;

    [Header("Rotation Settings")]
    [Tooltip("Minimum speed of ball's self-rotation")]
    [Range(0f, 100f)]
    public float minRotationSpeed = 20f;
    [Tooltip("Maximum speed of ball's self-rotation")]
    [Range(0f, 100f)]
    public float maxRotationSpeed = 50f;

    [Header("Orbit Settings")]
    [Tooltip("Minimum speed of ball's orbit movement")]
    [Range(0f, 100f)]
    public float minOrbitSpeed = 10f;
    [Tooltip("Maximum speed of ball's orbit movement")]
    [Range(0f, 100f)]
    public float maxOrbitSpeed = 30f;
    [Tooltip("Minimum radius of orbit")]
    [Range(0.1f, 10f)]
    public float minRadius = 1f;
    [Tooltip("Maximum radius of orbit")]
    [Range(0.1f, 10f)]
    public float maxRadius = 3f;

    [Header("Motion Control")]
    [Tooltip("Global speed multiplier")]
    [Range(0f, 2f)]
    public float speedMultiplier = 1f;
    [Tooltip("Focus value influence on rotation")]
    [Range(0f, 10f)]
    public float FocusValue = 0f;

    [Header("Distribution Pattern")]
    [Tooltip("Choose how balls are distributed")]
    public DistributionType distributionPattern = DistributionType.Random;

    public enum DistributionType
    {
        Random,
        Uniform,
        Spiral,
        Layers
    }

    private GameObject[] balls;
    private Vector3[] rotationAxes;
    private float[] rotationSpeeds;
    private float[] orbitSpeeds;
    private float[] orbitRadii;
    private float[] angleOffsets;

    void Start()
    {
        InitializeArrays();
    }

    void InitializeArrays()
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
        if (rotateBalls != null)
        {
            if (fcf == 0)
            {
                rotateBalls.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, InteraxonInterfacer.Instance.calm * 6));
            }
            if (fcf == 1)
            {
                rotateBalls.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, InteraxonInterfacer.Instance.focus * 6));
            }
            if (fcf == 2)
            {
                rotateBalls.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, InteraxonInterfacer.Instance.calm * 6));
            }
            if (fcf == 3)
            {
                rotateBalls.SetColor("_EmissionColor", Color.white * Mathf.Pow(2, InteraxonInterfacer.Instance.flow * 2));
            }

        }

        if (FinalSceneStart && (balls == null || balls[0] == null))
        {
            GenerateBalls();
        }

        if (FinalSceneStart && balls != null && balls[0] != null)
        {
            UpdateBallsMovement();
        }
    }

    void GenerateBalls()
    {
        InitializeArrays();

        switch (distributionPattern)
        {
            case DistributionType.Random:
                GenerateRandomDistribution();
                break;
            case DistributionType.Uniform:
                GenerateUniformDistribution();
                break;
            case DistributionType.Spiral:
                GenerateSpiralDistribution();
                break;
            case DistributionType.Layers:
                GenerateLayeredDistribution();
                break;
        }
    }

    void GenerateRandomDistribution()
    {
        for (int i = 0; i < BallsNumber; i++)
        {
            Vector3 position;
            bool validPosition;
            int attempts = 0;
            do
            {
                position = Random.insideUnitSphere * RadioBall;
                validPosition = CheckValidPosition(position, i);
                attempts++;
            } while (!validPosition && attempts < 100);

            CreateBall(i, position);
        }
    }

    void GenerateUniformDistribution()
    {
        for (int i = 0; i < BallsNumber; i++)
        {
            float phi = Mathf.Acos(1 - 2.0f * i / BallsNumber);
            float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

            Vector3 position = new Vector3(
                Mathf.Sin(phi) * Mathf.Cos(theta),
                Mathf.Sin(phi) * Mathf.Sin(theta),
                Mathf.Cos(phi)
            ) * RadioBall;

            CreateBall(i, position);
        }
    }

    void GenerateSpiralDistribution()
    {
        float angleStep = 360f / BallsNumber;
        float heightStep = RadioBall * 2 / BallsNumber;

        for (int i = 0; i < BallsNumber; i++)
        {
            float angle = angleStep * i;
            float height = -RadioBall + heightStep * i;
            float radius = Mathf.Lerp(minRadius, RadioBall, (float)i / BallsNumber);

            Vector3 position = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                height,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );

            CreateBall(i, position);
        }
    }

    void GenerateLayeredDistribution()
    {
        int layerCount = Mathf.CeilToInt(Mathf.Sqrt(BallsNumber));
        int ballsPerLayer = Mathf.CeilToInt((float)BallsNumber / layerCount);

        int currentBall = 0;
        for (int layer = 0; layer < layerCount && currentBall < BallsNumber; layer++)
        {
            float layerHeight = (((float)layer / (layerCount - 1)) - 0.5f) * RadioBall * 2;
            float layerRadius = Mathf.Sqrt(RadioBall * RadioBall - layerHeight * layerHeight);

            for (int i = 0; i < ballsPerLayer && currentBall < BallsNumber; i++)
            {
                float angle = ((float)i / ballsPerLayer) * Mathf.PI * 2;
                Vector3 position = new Vector3(
                    Mathf.Cos(angle) * layerRadius,
                    layerHeight,
                    Mathf.Sin(angle) * layerRadius
                );

                CreateBall(currentBall, position);
                currentBall++;
            }
        }
    }

    bool CheckValidPosition(Vector3 newPosition, int currentIndex)
    {
        for (int i = 0; i < currentIndex; i++)
        {
            if (balls[i] != null)
            {
                float distance = Vector3.Distance(newPosition, balls[i].transform.localPosition);
                if (distance < MinBallDistance)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void CreateBall(int index, Vector3 position)
    {
        balls[index] = Instantiate(RoatateBallPre, position + transform.position, Quaternion.identity);
        balls[index].transform.SetParent(transform);
        balls[index].name = $"Ball_{index}";

        rotationAxes[index] = Random.onUnitSphere;
        rotationSpeeds[index] = Random.Range(minRotationSpeed, maxRotationSpeed);
        orbitSpeeds[index] = Random.Range(minOrbitSpeed, maxOrbitSpeed);
        orbitRadii[index] = Random.Range(minRadius, maxRadius);
        angleOffsets[index] = Random.Range(0f, 360f);
    }

    void UpdateBallsMovement()
    {
        for (int i = 0; i < BallsNumber; i++)
        {
            if (balls[i] != null)
            {
                if(fcf == 1)
                {
                    speedMultiplier = InteraxonInterfacer.Instance.focus/2;
                }
                if (fcf == 2)
                {
                    speedMultiplier = InteraxonInterfacer.Instance.calm/2;
                }
                if (fcf == 3)
                {
                    speedMultiplier = InteraxonInterfacer.Instance.flow/6;
                }

                float currentRotationSpeed = rotationSpeeds[i] * speedMultiplier + InteraxonInterfacer.Instance.focus * FocusValue;
                balls[i].transform.Rotate(rotationAxes[i] * currentRotationSpeed * Time.deltaTime);

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