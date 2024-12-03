using UnityEngine;
using System.Collections.Generic;

public class PointModelInteractor : MonoBehaviour
{
    [System.Serializable]
    public class PointData
    {
        public Vector3 position;
        public Vector3 originalPosition; // 存储原始位置
        public Color color;
        public float size;
        public bool selected;
        public float oscillationOffset; // 每个点的振荡偏移
        public float sizeOffset; // 大小变化的偏移
    }

    public GameObject pointPrefab;
    public float selectionRadius = 0.5f;
    public Color selectedColor = Color.red;

    // 动画参数
    public float oscillationSpeed = 1f; // 振荡速度
    public float oscillationHeight = 0.5f; // 振荡高度
    public float sizeChangeSpeed = 0.5f; // 大小变化速度
    public float minSize = 0.05f; // 最小尺寸
    public float maxSize = 0.15f; // 最大尺寸

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
        // 处理点的动画
        AnimatePoints();

        // 处理鼠标点击选择
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                SelectPointsInRadius(hit.point);
            }
        }

        // 处理选中点的移动
        if (Input.GetKey(KeyCode.G))
        {
            MoveSelectedPoints();
        }
    }

    void LoadPointCloudData()
    {
        // 创建测试点
        for (int i = 0; i < 1000; i++)
        {
            Vector3 pos = Random.insideUnitSphere * 10f;
            PointData point = new PointData
            {
                position = pos,
                originalPosition = pos, // 保存原始位置
                color = Color.white,
                size = Random.Range(minSize, maxSize),
                selected = false,
                oscillationOffset = Random.Range(0f, 2f * Mathf.PI), // 随机初始相位
                sizeOffset = Random.Range(0f, 2f * Mathf.PI) // 随机大小变化相位
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
            if (!points[i].selected) // 只有未选中的点才会动画
            {
                // 计算高度偏移
                float heightOffset = Mathf.Sin(time * oscillationSpeed + points[i].oscillationOffset) * oscillationHeight;

                // 更新位置
                Vector3 newPosition = points[i].originalPosition + Vector3.up * heightOffset;
                points[i].position = newPosition;
                pointObjects[i].transform.position = newPosition;

                // 计算大小变化
                float sizeMultiplier = Mathf.Sin(time * sizeChangeSpeed + points[i].sizeOffset) * 0.5f + 1.5f;
                float newSize = points[i].size * sizeMultiplier;
                newSize = Mathf.Clamp(newSize, minSize, maxSize);

                // 更新大小
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
                points[i].originalPosition += moveDirection; // 更新原始位置
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