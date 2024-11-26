using UnityEngine;
using System.Collections.Generic;

public class PointCloudGPUInstancing : MonoBehaviour
{
    // 点云数据结构
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

    // 配置参数
    public Mesh pointMesh; // 点的mesh（通常是一个小球体）
    public Material pointMaterial; // 支持GPU Instancing的材质
    public int maxInstancesPerBatch = 1000; // 每批次最大实例数

    // 实例化数据
    private List<PointData> points = new List<PointData>();
    private List<Matrix4x4> matrices = new List<Matrix4x4>();
    private List<Vector4> colors = new List<Vector4>();
    private MaterialPropertyBlock propertyBlock;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer colorBuffer;

    void Start()
    {
        // 确保材质启用GPU Instancing
        pointMaterial.enableInstancing = true;
        propertyBlock = new MaterialPropertyBlock();

        // 生成示例点云数据
        GenerateExamplePointCloud();

        // 初始化缓冲区
        InitializeBuffers();
    }

    void GenerateExamplePointCloud()
    {
        // 生成示例点云（10万个点）
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
        // 创建计算缓冲区
        if (positionBuffer != null) positionBuffer.Release();
        if (colorBuffer != null) colorBuffer.Release();

        positionBuffer = new ComputeBuffer(points.Count, sizeof(float) * 16); // Matrix4x4 size
        colorBuffer = new ComputeBuffer(points.Count, sizeof(float) * 4); // Vector4 size

        // 更新缓冲区数据
        positionBuffer.SetData(matrices.ToArray());
        colorBuffer.SetData(colors.ToArray());

        // 设置材质属性
        propertyBlock.SetBuffer("_PositionBuffer", positionBuffer);
        propertyBlock.SetBuffer("_ColorBuffer", colorBuffer);
    }

    void Update()
    {
        // 分批次渲染实例
        int remaining = points.Count;
        int offset = 0;

        while (remaining > 0)
        {
            int batchCount = Mathf.Min(remaining, maxInstancesPerBatch);

            // 使用GPU Instancing绘制一批次
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
        // 释放缓冲区
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

    // 更新点的位置或颜色
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

        // 更新缓冲区
        positionBuffer.SetData(matrices.ToArray());
        colorBuffer.SetData(colors.ToArray());
    }
}