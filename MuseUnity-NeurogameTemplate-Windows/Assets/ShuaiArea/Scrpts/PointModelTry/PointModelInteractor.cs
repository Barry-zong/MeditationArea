using UnityEngine;
using System.Collections.Generic;

public class PointModelInteractor : MonoBehaviour
{
    [System.Serializable]
    public class PointData
    {
        public Vector3 position;
        public Vector3 originalPosition; // �洢ԭʼλ��
        public Color color;
        public float size;
        public bool selected;
        public float oscillationOffset; // ÿ�������ƫ��
        public float sizeOffset; // ��С�仯��ƫ��
    }

    public GameObject pointPrefab;
    public float selectionRadius = 0.5f;
    public Color selectedColor = Color.red;

    // ��������
    public float oscillationSpeed = 1f; // ���ٶ�
    public float oscillationHeight = 0.5f; // �񵴸߶�
    public float sizeChangeSpeed = 0.5f; // ��С�仯�ٶ�
    public float minSize = 0.05f; // ��С�ߴ�
    public float maxSize = 0.15f; // ���ߴ�

    private List<PointData> points = new List<PointData>();
    private List<GameObject> pointObjects = new List<GameObject>();
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        LoadPointCloudData();
    }

    void Update()
    {
        // �����Ķ���
        AnimatePoints();

        // ���������ѡ��
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                SelectPointsInRadius(hit.point);
            }
        }

        // ����ѡ�е���ƶ�
        if (Input.GetKey(KeyCode.G))
        {
            MoveSelectedPoints();
        }
    }

    void LoadPointCloudData()
    {
        // �������Ե�
        for (int i = 0; i < 1000; i++)
        {
            Vector3 pos = Random.insideUnitSphere * 10f;
            PointData point = new PointData
            {
                position = pos,
                originalPosition = pos, // ����ԭʼλ��
                color = Color.white,
                size = Random.Range(minSize, maxSize),
                selected = false,
                oscillationOffset = Random.Range(0f, 2f * Mathf.PI), // �����ʼ��λ
                sizeOffset = Random.Range(0f, 2f * Mathf.PI) // �����С�仯��λ
            };
            points.Add(point);

            GameObject pointObj = Instantiate(pointPrefab, point.position, Quaternion.identity);
            pointObj.transform.localScale = Vector3.one * point.size;
            pointObj.GetComponent<Renderer>().material.color = point.color;
            pointObjects.Add(pointObj);
        }
    }

    void AnimatePoints()
    {
        float time = Time.time;

        for (int i = 0; i < points.Count; i++)
        {
            if (!points[i].selected) // ֻ��δѡ�еĵ�Żᶯ��
            {
                // ����߶�ƫ��
                float heightOffset = Mathf.Sin(time * oscillationSpeed + points[i].oscillationOffset) * oscillationHeight;

                // ����λ��
                Vector3 newPosition = points[i].originalPosition + Vector3.up * heightOffset;
                points[i].position = newPosition;
                pointObjects[i].transform.position = newPosition;

                // �����С�仯
                float sizeMultiplier = Mathf.Sin(time * sizeChangeSpeed + points[i].sizeOffset) * 0.5f + 1.5f;
                float newSize = points[i].size * sizeMultiplier;
                newSize = Mathf.Clamp(newSize, minSize, maxSize);

                // ���´�С
                pointObjects[i].transform.localScale = Vector3.one * newSize;
            }
        }
    }

    void SelectPointsInRadius(Vector3 center)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (Vector3.Distance(points[i].position, center) < selectionRadius)
            {
                points[i].selected = !points[i].selected;
                pointObjects[i].GetComponent<Renderer>().material.color =
                    points[i].selected ? selectedColor : points[i].color;
            }
        }
    }

    void MoveSelectedPoints()
    {
        Vector3 moveDirection = new Vector3(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            0
        ) * Time.deltaTime;

        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].selected)
            {
                points[i].position += moveDirection;
                points[i].originalPosition += moveDirection; // ����ԭʼλ��
                pointObjects[i].transform.position = points[i].position;
            }
        }
    }

    public void AddPoint(Vector3 position)
    {
        PointData newPoint = new PointData
        {
            position = position,
            originalPosition = position,
            color = Color.white,
            size = Random.Range(minSize, maxSize),
            selected = false,
            oscillationOffset = Random.Range(0f, 2f * Mathf.PI),
            sizeOffset = Random.Range(0f, 2f * Mathf.PI)
        };
        points.Add(newPoint);

        GameObject pointObj = Instantiate(pointPrefab, position, Quaternion.identity);
        pointObj.transform.localScale = Vector3.one * newPoint.size;
        pointObj.GetComponent<Renderer>().material.color = newPoint.color;
        pointObjects.Add(pointObj);
    }

    public void DeleteSelectedPoints()
    {
        for (int i = points.Count - 1; i >= 0; i--)
        {
            if (points[i].selected)
            {
                Destroy(pointObjects[i]);
                pointObjects.RemoveAt(i);
                points.RemoveAt(i);
            }
        }
    }
}