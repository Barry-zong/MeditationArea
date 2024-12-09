using UnityEngine;

public class RandomRotatingBalls : MonoBehaviour
{
    public bool FinalSceneStart = false;
    public float RadioBall = 5f; // ��������İ뾶��Χ
    public int BallsNumber = 10; // Ҫ���ɵ���������
    public GameObject RoatateBallPre; // ����Ԥ����

    // �˶����Ʋ���
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
            // �������������������λ��
            Vector3 randomPosition = Random.insideUnitSphere * RadioBall;
            balls[i] = Instantiate(RoatateBallPre, randomPosition + transform.position, Quaternion.identity);
            balls[i].transform.SetParent(transform);

            // ��ʼ������˶�����
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
                // ��ת�˶�
                balls[i].transform.Rotate(rotationAxes[i] * rotationSpeeds[i] * Time.deltaTime);

                // ��ת�˶�
                float angle = Time.time * orbitSpeeds[i] + angleOffsets[i];
                Vector3 orbitPosition = new Vector3(
                    Mathf.Cos(angle) * orbitRadii[i],
                    Mathf.Sin(angle * 0.5f) * orbitRadii[i], // ʹ�ò�ͬ�ĽǶ�ϵ�����첻�����˶�
                    Mathf.Sin(angle) * orbitRadii[i]
                );
                balls[i].transform.position = transform.position + orbitPosition;
            }
        }
    }

    // �������ɵ�����
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