using UnityEngine;
using System.Collections.Generic;

public class FinalInteractParticleGen : MonoBehaviour
{
    [System.Serializable]
    public class PointData
    {
        public Vector3 position;
        public Vector3 originalPosition;
        public Color color;
        public float baseSize; // ������С
        public float oscillationOffset;
    }

    public GameObject pointPrefab;
    public float oscillationSpeed = 1f;
    public float oscillationHeight = 0.5f;
    public float minSize = 0.05f;
    public float maxSize = 0.15f;
    public bool isDebug = false;
    public float FocusMimicValue = 0;
    public float spawnRadius = 10f; // ���ɷ�Χ�İ뾶

    private float focus = 0;
    private List<PointData> points = new List<PointData>();
    private List<GameObject> pointObjects = new List<GameObject>();

    void Start()
    {
        LoadPointCloudData();
    }

    void Update()
    {
        AnimatePoints();
    }

    void LoadPointCloudData()
    {
        Vector3 origin = transform.position; // ��ȡ���ض����λ����Ϊԭ��

        for (int i = 0; i < 1000; i++)
        {
            // ��ԭ��Ϊ�������ɵ�
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            Vector3 pos = origin + randomOffset;

            PointData point = new PointData
            {
                position = pos,
                originalPosition = pos,
                color = Color.white,
                baseSize = Random.Range(minSize, maxSize),
                oscillationOffset = Random.Range(0f, 2f * Mathf.PI)
            };
            points.Add(point);

            GameObject pointObj = Instantiate(pointPrefab, point.position, Quaternion.identity);
            pointObj.transform.parent = transform; // �����ɵĵ���Ϊ���ض����������
            pointObj.transform.localScale = Vector3.one * point.baseSize;
            pointObj.GetComponent<Renderer>().material.color = point.color;
            pointObjects.Add(pointObj);
        }
    }

    void AnimatePoints()
    {
        float time = Time.time;
        if (isDebug)
        {
            focus = FocusMimicValue;
        }
        else
        {
            focus = InteraxonInterfacer.Instance.focus;
        }
        float sizeMultiplier = Mathf.Lerp(0, 1, focus / 0.3f); // ��focusΪ0.3ʱ�ﵽԭʼ��С

        for (int i = 0; i < points.Count; i++)
        {
            // ����߶�ƫ��
            float heightOffset = Mathf.Sin(time * oscillationSpeed + points[i].oscillationOffset) * oscillationHeight;

            // ����λ��
            Vector3 newPosition = points[i].originalPosition + Vector3.up * heightOffset;
            points[i].position = newPosition;
            pointObjects[i].transform.position = newPosition;

            // ���´�С������focusֵ
            float newSize = points[i].baseSize * sizeMultiplier;
            newSize = Mathf.Clamp(newSize, 0, points[i].baseSize);
            pointObjects[i].transform.localScale = Vector3.one * newSize;
        }
    }

    public void AddPoint(Vector3 position)
    {
        PointData newPoint = new PointData
        {
            position = position,
            originalPosition = position,
            color = Color.white,
            baseSize = Random.Range(minSize, maxSize),
            oscillationOffset = Random.Range(0f, 2f * Mathf.PI)
        };
        points.Add(newPoint);

        GameObject pointObj = Instantiate(pointPrefab, position, Quaternion.identity);
        pointObj.transform.parent = transform; // ����Ϊ���ض����������
        pointObj.transform.localScale = Vector3.one * newPoint.baseSize;
        pointObj.GetComponent<Renderer>().material.color = newPoint.color;
        pointObjects.Add(pointObj);
    }
}