using UnityEngine;
using System.Collections.Generic;

public class PointModelInteractor : MonoBehaviour
{
    [System.Serializable]
    public class PointData
    {
        public Vector3 position;
        public Color color;
        public float size;
        public bool selected;
    }

    public GameObject pointPrefab; // 单个点的预制体
    public float selectionRadius = 0.5f; // 选择点的半径
    public Color selectedColor = Color.red; // 选中点的颜色

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
        // 示例：创建一些测试点
        for (int i = 0; i < 1000; i++)
        {
            PointData point = new PointData
            {
                position = Random.insideUnitSphere * 10f,
                color = Color.white,
                size = 0.1f,
                selected = false
            };
            points.Add(point);

            GameObject pointObj = Instantiate(pointPrefab, point.position, Quaternion.identity);
            pointObj.transform.localScale = Vector3.one * point.size;
            pointObj.GetComponent<Renderer>().material.color = point.color;
            pointObjects.Add(pointObj);
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
                pointObjects[i].transform.position = points[i].position;
            }
        }
    }

    // 保存点云数据
    public void SavePointCloudData()
    {
        // 实现保存逻辑
        // 可以保存为自定义格式或标准点云格式（如PLY、PCD等）
    }

    // 加载点云数据
    public void LoadExternalPointCloud(string filePath)
    {
        // 实现加载外部点云文件的逻辑
        // 支持常见点云格式的解析
    }

    // 添加新的点
    public void AddPoint(Vector3 position)
    {
        PointData newPoint = new PointData
        {
            position = position,
            color = Color.white,
            size = 0.1f,
            selected = false
        };
        points.Add(newPoint);

        GameObject pointObj = Instantiate(pointPrefab, position, Quaternion.identity);
        pointObj.transform.localScale = Vector3.one * newPoint.size;
        pointObj.GetComponent<Renderer>().material.color = newPoint.color;
        pointObjects.Add(pointObj);
    }

    // 删除选中的点
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
