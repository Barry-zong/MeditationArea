using UnityEngine;
using System.Collections.Generic;

public class PointCloudGPUInstancing : MonoBehaviour
{
    // �������ݽṹ
    public struct PointData
    {
        public Vector3 position;
        public Vector3 scale;
        public Vector4 color;
        public Matrix4x4 matrix;

        public PointData(Vector3 pos, Vector3 scl, Color col)
        {
            position = pos;
            scale = scl;
            color = new Vector4(col.r, col.g, col.b, col.a);
            matrix = Matrix4x4.TRS(position, Quaternion.identity, scale);
        }
    }

    // ���ò���
    public Mesh pointMesh; // ���mesh��ͨ����һ��С���壩
    public Material pointMaterial; // ֧��GPU Instancing�Ĳ���
    public int maxInstancesPerBatch = 1000; // ÿ�������ʵ����

    // ʵ��������
    private List<PointData> points = new List<PointData>();
    private List<Matrix4x4> matrices = new List<Matrix4x4>();
    private List<Vector4> colors = new List<Vector4>();
    private MaterialPropertyBlock propertyBlock;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer colorBuffer;

    void Start()
    {
        // ȷ����������GPU Instancing
        pointMaterial.enableInstancing = true;
        propertyBlock = new MaterialPropertyBlock();

        // ����ʾ����������
        GenerateExamplePointCloud();

        // ��ʼ��������
        InitializeBuffers();
    }

    void GenerateExamplePointCloud()
    {
        // ����ʾ�����ƣ�10����㣩
        for (int i = 0; i < 100000; i++)
        {
            Vector3 position = Random.insideUnitSphere * 10f;
            Vector3 scale = Vector3.one * 0.05f;
            Color color = Color.HSVToRGB(Random.value, 0.7f, 1f);

            points.Add(new PointData(position, scale, color));
            matrices.Add(Matrix4x4.TRS(position, Quaternion.identity, scale));
            colors.Add(new Vector4(color.r, color.g, color.b, color.a));
        }
    }

    void InitializeBuffers()
    {
        // �������㻺����
        if (positionBuffer != null) positionBuffer.Release();
        if (colorBuffer != null) colorBuffer.Release();

        positionBuffer = new ComputeBuffer(points.Count, sizeof(float) * 16); // Matrix4x4 size
        colorBuffer = new ComputeBuffer(points.Count, sizeof(float) * 4); // Vector4 size

        // ���»���������
        positionBuffer.SetData(matrices.ToArray());
        colorBuffer.SetData(colors.ToArray());

        // ���ò�������
        propertyBlock.SetBuffer("_PositionBuffer", positionBuffer);
        propertyBlock.SetBuffer("_ColorBuffer", colorBuffer);
    }

    void Update()
    {
        // ��������Ⱦʵ��
        int remaining = points.Count;
        int offset = 0;

        while (remaining > 0)
        {
            int batchCount = Mathf.Min(remaining, maxInstancesPerBatch);

            // ʹ��GPU Instancing����һ����
            Graphics.DrawMeshInstanced(
                pointMesh,
                0,
                pointMaterial,
                matrices.GetRange(offset, batchCount).ToArray(),
                batchCount,
                propertyBlock
            );

            offset += batchCount;
            remaining -= batchCount;
        }
    }

    void OnDestroy()
    {
        // �ͷŻ�����
        if (positionBuffer != null)
        {
            positionBuffer.Release();
            positionBuffer = null;
        }
        if (colorBuffer != null)
        {
            colorBuffer.Release();
            colorBuffer = null;
        }
    }

    // ���µ��λ�û���ɫ
    public void UpdatePoint(int index, Vector3 newPosition, Color newColor)
    {
        if (index < 0 || index >= points.Count) return;

        PointData point = points[index];
        point.position = newPosition;
        point.color = new Vector4(newColor.r, newColor.g, newColor.b, newColor.a);
        point.matrix = Matrix4x4.TRS(newPosition, Quaternion.identity, point.scale);

        points[index] = point;
        matrices[index] = point.matrix;
        colors[index] = point.color;

        // ���»�����
        positionBuffer.SetData(matrices.ToArray());
        colorBuffer.SetData(colors.ToArray());
    }
}