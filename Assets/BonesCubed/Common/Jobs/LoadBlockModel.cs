using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Bones3.Native.Unsafe;

namespace Bones3.Jobs
{
  /// <summary>
  /// Converts a standard Unity mesh object into a NativeBlockModel instance.
  /// </summary>
  [BurstCompile]
  public struct LoadBlockModel : IJobParallelFor
  {
    /// <summary>
    /// The mesh data of the Unity mesh object to read the vertex data from.
    /// </summary>
    [ReadOnly]
    public Mesh.MeshDataArray meshData;


    /// <summary>
    /// The array of generated native block models to add the mesh data to.
    /// </summary>
    public NativeArray<UnsafeBlockModel> models;


    /// <inheritdoc/>
    [BurstCompile]
    public void Execute(int modelIndex)
    {
      var mesh = this.meshData[modelIndex];
      var vertexCount = mesh.vertexCount;
      var indexCount = mesh.GetSubMesh(0).indexCount;
      var vertices = new NativeArray<Vector3>(vertexCount, Allocator.Temp);
      var normals = new NativeArray<Vector3>(vertexCount, Allocator.Temp);
      var uvs = new NativeArray<Vector3>(vertexCount, Allocator.Temp);
      var tangents = new NativeArray<Vector4>(vertexCount, Allocator.Temp);
      var indices = new NativeArray<int>(indexCount, Allocator.Temp);
      mesh.GetVertices(vertices);
      mesh.GetNormals(normals);
      mesh.GetUVs(0, uvs);
      mesh.GetTangents(tangents);
      mesh.GetIndices(indices, 0);

      var voxelVerts = new NativeArray<VoxelVertex>(vertexCount, Allocator.Temp);
      for (int i = 0; i < vertexCount; i++)
      {
        voxelVerts[i] = new VoxelVertex()
        {
          position = vertices[i],
          normal = normals[i],
          tangent = tangents[i],
          uv = uvs[i]
        };
      }
      vertices.Dispose();
      normals.Dispose();
      uvs.Dispose();
      tangents.Dispose();

      var usedSegments = IBlockModel.OccludingSegment.None;
      var segmentIndicators = new NativeArray<IBlockModel.OccludingSegment>(indexCount / 3, Allocator.Temp);
      for (int i = 0; i < indexCount; i += 3)
      {
        var v0 = voxelVerts[indices[i + 0]];
        var v1 = voxelVerts[indices[i + 1]];
        var v2 = voxelVerts[indices[i + 2]];
        var segment = CalculateSegment(v0.position, v1.position, v2.position);
        segmentIndicators[i / 3] = segment;
        usedSegments |= segment;
      }

      var blockModel = this.models[modelIndex];

      var segmentVerts = new NativeList<VoxelVertex>(256, Allocator.Temp);
      var segmentIndis = new NativeList<ushort>(256, Allocator.Temp);
      for (int i = 1; i < (int)IBlockModel.OccludingSegment.Everything; i <<= 1)
      {
        var segment = (IBlockModel.OccludingSegment)i;
        if ((segment & usedSegments) == 0) continue;

        for (int j = 0; j < segmentIndicators.Length; j++)
        {
          if (segmentIndicators[j] != segment) continue;

          AppendIndex(segmentVerts, segmentIndis, voxelVerts[indices[j * 3 + 0]]);
          AppendIndex(segmentVerts, segmentIndis, voxelVerts[indices[j * 3 + 1]]);
          AppendIndex(segmentVerts, segmentIndis, voxelVerts[indices[j * 3 + 2]]);
        }

        blockModel.AddSegment(segmentVerts, segmentIndis, segment);
        segmentVerts.Length = 0;
        segmentIndis.Length = 0;
      }

      this.models[modelIndex] = blockModel;

      segmentVerts.Dispose();
      segmentIndis.Dispose();
      voxelVerts.Dispose();
      indices.Dispose();
    }


    /// <summary>
    /// Appends the index of the provided vertex to the given index list, adding
    /// the vertex to the vertex list if needed.
    /// </summary>
    /// <param name="vertices">The vertex list.</param>
    /// <param name="indices">The index list.</param>
    /// <param name="vertex">The vertex.</param>
    [BurstCompile]
    private void AppendIndex(NativeList<VoxelVertex> vertices, NativeList<ushort> indices, VoxelVertex vertex)
    {
      var index = GetVertexIndex(vertices, vertex);
      if (index < 0)
      {
        vertices.Add(vertex);
        index = vertices.Length - 1;
      }

      indices.Add((ushort)index);
    }


    /// <summary>
    /// Gets the index of the given vertex within the list.
    /// </summary>
    /// <param name="list">The list to look through.</param>
    /// <param name="vertex">The vertex to look for.</param>
    /// <returns>The index of the vertex in the list, or -1 if not present.</returns>
    [BurstCompile]
    private int GetVertexIndex(NativeList<VoxelVertex> list, VoxelVertex vertex)
    {
      for (int i = list.Length - 1; i >= 0; i--)
        if (list[i] == vertex) return i;

      return -1;
    }


    /// <summary>
    /// Calculates the block model occluding segment that the given triangle is
    /// in.
    /// </summary>
    /// <param name="v0">The first vertex position.</param>
    /// <param name="v1">The second vertex position.</param>
    /// <param name="v2">The third vertex position.</param>
    /// <returns>The calculated segment.</returns>
    [BurstCompile]
    private IBlockModel.OccludingSegment CalculateSegment(float3 v0, float3 v1, float3 v2)
    {
      var position = (v0 + v1 + v2) / 3;
      var normal = math.normalize(math.cross(v1 - v0, v2 - v0));

      var segment = IBlockModel.OccludingSegment.Center;
      if (math.dot(normal, new float3(0, 0, 1)) > 0.9999 && Mathf.Abs(position.z - 1) < 0.0001) segment = IBlockModel.OccludingSegment.North;
      if (math.dot(normal, new float3(1, 0, 0)) > 0.9999 && Mathf.Abs(position.x - 1) < 0.0001) segment = IBlockModel.OccludingSegment.East;
      if (math.dot(normal, new float3(0, 0, -1)) > 0.9999 && Mathf.Abs(position.z - 0) < 0.0001) segment = IBlockModel.OccludingSegment.South;
      if (math.dot(normal, new float3(-1, 0, 0)) > 0.9999 && Mathf.Abs(position.x - 0) < 0.0001) segment = IBlockModel.OccludingSegment.West;
      if (math.dot(normal, new float3(0, 1, 0)) > 0.9999 && Mathf.Abs(position.y - 1) < 0.0001) segment = IBlockModel.OccludingSegment.Top;
      if (math.dot(normal, new float3(0, -1, 0)) > 0.9999 && Mathf.Abs(position.y - 0) < 0.0001) segment = IBlockModel.OccludingSegment.Bottom;
      return segment;
    }
  }
}
