using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Bones3.Jobs;
using Bones3.Native;
using Bones3.Native.Unsafe;

namespace Bones3.Util
{
  /// <summary>
  /// A collection of utility functions for working with mesh data.
  /// </summary>
  public static class MeshUtilities
  {
    /// <summary>
    /// Loads an array of block models into native block models to be used
    /// within Bones3 mesh generation tasks. Note that only the first submesh
    /// of each block model is used.
    /// </summary>
    /// <param name="models">The models to load.</param>
    /// <param name="generatedModels">The native array of generated models.</param>
    /// <param name="meshData">The mesh data container being used by this job. Needs to be disposed manually when the job is complete.</param>
    /// <returns>The job handle responsible for loading all native block models.</returns>
    public static JobHandle LoadBlockModels(IBlockModel[] models, out NativeArray<UnsafeBlockModel> generatedModels, out Mesh.MeshDataArray meshData)
    {
      meshData = Mesh.AcquireReadOnlyMeshData(models.Select(m => m.Mesh).ToArray());
      generatedModels = new NativeArray<UnsafeBlockModel>(models.Length, Allocator.Persistent);
      for (int i = 0; i < models.Length; i++) generatedModels[i] = new UnsafeBlockModel(models[i].OccludingDirections, Allocator.Persistent);

      return new LoadBlockModel()
      {
        models = generatedModels,
        meshData = meshData
      }.Schedule(models.Length, 4);
    }


    /// <summary>
    /// Generates a new mesh for the given region of an infinite grid of model
    /// IDs.
    /// </summary>
    /// <param name="region">The region to generate a mesh for.</param>
    /// <param name="modelIds">The infinite grid of model Ids.</param>
    /// <param name="materialIds">The infinite grid of material Ids.</param>
    /// <param name="blockModels">The list of block models to read mesh data from.</param>
    /// <param name="meshData">The generated mesh data. (After job completion.)</param>
    /// <param name="materialIndices">The list of material indices for each submesh. (After job completion.)</param>
    /// <returns>The job handle containing this remesh task.</returns>
    public static JobHandle RemeshRegion(Region region, NativeInfiniteGrid3D<ushort> modelIds, NativeInfiniteGrid3D<ushort> materialIds, NativeArray<UnsafeBlockModel> blockModels, out Mesh.MeshDataArray meshData, out NativeList<int> materialIndices)
    {
      var blockVisibility = new NativeGrid3D<IBlockModel.OccludingSegment>(region, Allocator.TempJob);
      meshData = Mesh.AllocateWritableMeshData(1);
      materialIndices = new NativeList<int>(4, Allocator.TempJob);

      var jobHandle = new CalculateBlockOcclusion()
      {
        models = blockModels,
        modelIds = modelIds,
        blockVisiblity = blockVisibility,
      }.Schedule(blockVisibility.Length, 16);

      jobHandle = new GenerateRegionMesh()
      {
        models = blockModels,
        modelIds = modelIds,
        materialIds = materialIds,
        blockVisibility = blockVisibility,
        generatedMesh = meshData[0],
        materialIndices = materialIndices,
      }.Schedule(jobHandle);

      jobHandle = blockVisibility.Dispose(jobHandle);
      return jobHandle;
    }
  }
}
