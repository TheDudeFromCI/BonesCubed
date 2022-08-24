using UnityEngine;
using UnityEngine.Rendering;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Bones3.Native;
using Bones3.Native.Unsafe;

namespace Bones3.Jobs
{
  [BurstCompile]
  public struct GenerateRegionMesh : IJob
  {
    /// <summary>
    /// The list of block models to read the mesh data from.
    /// </summary>
    [ReadOnly]
    public NativeArray<UnsafeBlockModel> models;


    /// <summary>
    /// The model ids of the world that contains the target region.
    /// </summary>
    [ReadOnly]
    public NativeInfiniteGrid3D<ushort> modelIds;


    /// <summary>
    /// The material ids of the world that contains the target region.
    /// </summary>
    [ReadOnly]
    public NativeInfiniteGrid3D<ushort> materialIds;


    /// <summary>
    /// The input occlusion and visiblity data for each block within a region of
    /// the infinite modelIds grid.
    /// </summary>
    [ReadOnly]
    public NativeGrid3D<IBlockModel.OccludingSegment> blockVisibility;


    /// <summary>
    /// The mesh data to write the generated region mesh information to.
    /// </summary>
    public Mesh.MeshData generatedMesh;


    /// <summary>
    /// The native list of material pointers for the generated output mesh. Each
    /// item in this array indicated the index of the material used for the
    /// submesh at that location in the array.
    /// </summary>
    public NativeList<int> materialIndices;


    /// <inheritdoc/>
    [BurstCompile]
    public void Execute()
    {
      var meshVertices = new NativeList<VoxelVertex>(1024, Allocator.Temp);
      var meshIndices = new NativeList<UnsafeList<ushort>>(4, Allocator.Temp);

      var tempVerts = new NativeList<VoxelVertex>(64, Allocator.Temp);
      var tempIndis = new NativeList<ushort>(64, Allocator.Temp);

      var region = this.blockVisibility.Region;
      for (int i = 0; i < region.Length; i++)
      {
        var pos = region.PositionFromIndex(i);
        var visibleSegments = this.blockVisibility.GetElement(pos);
        if (visibleSegments == IBlockModel.OccludingSegment.None) continue;

        var modelIndex = this.modelIds.GetElement(pos) - 1;
        if (modelIndex < 0) continue;
        var model = this.models[modelIndex];

        var submesh = GetSubmesh(this.materialIds.GetElement(pos));
        while (meshIndices.Length <= submesh) meshIndices.Add(new UnsafeList<ushort>(64, Allocator.Temp));
        var submeshIndices = meshIndices[submesh];

        for (int j = 1; j < (int)IBlockModel.OccludingSegment.Everything; j <<= 1)
        {
          var segment = (IBlockModel.OccludingSegment)j;
          model.GetSegment(ref tempVerts, ref tempIndis, segment);

          int vertexOffset = meshVertices.Length;
          for (int k = 0; k < tempVerts.Length; k++)
          {
            var vertex = tempVerts[k];
            vertex.position += pos;
            meshVertices.Add(vertex);
          }

          for (int k = 0; k < tempIndis.Length; k++)
            submeshIndices.Add((ushort)(tempIndis[k] + vertexOffset));

          tempVerts.Length = 0;
          tempIndis.Length = 0;
        }

        meshIndices[submesh] = submeshIndices;
      }

      int totalIndices = 0;
      for (int i = 0; i < meshIndices.Length; i++) totalIndices += meshIndices[i].Length;

      var layout = new VoxelVertex().GetLayout();
      this.generatedMesh.subMeshCount = meshIndices.Length;
      this.generatedMesh.SetVertexBufferParams(meshVertices.Length, layout);
      this.generatedMesh.SetIndexBufferParams(totalIndices, IndexFormat.UInt16);

      var meshVertexData = this.generatedMesh.GetVertexData<VoxelVertex>();
      meshVertexData.CopyFrom(meshVertices);

      int index = 0;
      var meshIndexData = this.generatedMesh.GetIndexData<ushort>();
      for (int i = 0; i < meshIndices.Length; i++)
      {
        var startIndex = index;
        var indices = meshIndices[i];
        for (int j = 0; j < indices.Length; j++) meshIndexData[index++] = indices[j];
        this.generatedMesh.SetSubMesh(i, new SubMeshDescriptor(startIndex, indices.Length));
      }

      for (int i = 0; i < meshIndices.Length; i++) meshIndices[i].Dispose();
      meshVertices.Dispose();
      meshIndices.Dispose();
      tempVerts.Dispose();
      tempIndis.Dispose();
    }


    /// <summary>
    /// Gets the submesh for the given material id. This method will append new
    /// material ids to the materialIndices list as needed.
    /// </summary>
    /// <param name="materialId">The block's material id.</param>
    /// <returns>The submesh index.</returns>
    [BurstCompile]
    private int GetSubmesh(int materialId)
    {
      for (int i = 0; i < this.materialIndices.Length; i++)
        if (this.materialIndices[i] == materialId) return i;

      this.materialIndices.Add(materialId);
      return this.materialIndices.Length - 1;
    }
  }
}
