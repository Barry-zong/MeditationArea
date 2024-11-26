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

    public GameObject pointPrefab; // �������Ԥ����
    public float selectionRadius = 0.5f; // ѡ���İ뾶
    public Color selectedColor = Color.red; // ѡ�е����ɫ

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
        // ʾ��������һЩ���Ե�
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

    // �����������
    public void SavePointCloudData()
    {
        // ʵ�ֱ����߼�
        // ���Ա���Ϊ�Զ����ʽ���׼���Ƹ�ʽ����PLY��PCD�ȣ�
    }

    // ���ص�������
    public void LoadExternalPointCloud(string filePath)
    {
        // ʵ�ּ����ⲿ�����ļ����߼�
        // ֧�ֳ������Ƹ�ʽ�Ľ���
    }

    // ����µĵ�
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

    // ɾ��ѡ�еĵ�
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
