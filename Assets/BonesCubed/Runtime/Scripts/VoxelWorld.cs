using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Bones3.Native;
using Bones3.Native.Unsafe;
using Bones3.Util;

namespace Bones3.Runtime
{
  public class VoxelWorld : MonoBehaviour
  {
    public BlockType blockType;


    void Start()
    {
      IBlockModel[] blockModels = new[] { blockType.BlockModel };
      Material[] blockMaterials = new[] { blockType.Material };

      NativeArray<UnsafeBlockModel> models;
      Mesh.MeshDataArray meshDataArray;
      MeshUtilities.LoadBlockModels(blockModels, out models, out meshDataArray).Complete();
      meshDataArray.Dispose();

      var modelIds = new NativeInfiniteGrid3D<ushort>(Allocator.Persistent);
      modelIds.SetElement(0, 1);
      modelIds.SetElement(new int3(0, 0, 1), 1);

      var materialIds = new NativeInfiniteGrid3D<ushort>(Allocator.Persistent);

      var region = new Region(new int3(-5), new int3(11));
      Mesh.MeshDataArray meshData;
      NativeList<int> materialIndices;
      MeshUtilities.RemeshRegion(region, modelIds, materialIds, models, out meshData, out materialIndices).Complete();

      var mesh = new Mesh();
      Mesh.ApplyAndDisposeWritableMeshData(meshData, mesh);

      var sharedMaterials = new Material[materialIndices.Length];
      for (int i = 0; i < sharedMaterials.Length; i++) sharedMaterials[i] = blockMaterials[materialIndices[i]];
      materialIndices.Dispose();

      var go = new GameObject();
      go.AddComponent<MeshFilter>().sharedMesh = mesh;
      go.AddComponent<MeshRenderer>().sharedMaterials = sharedMaterials;

      for (int i = 0; i < models.Length; i++) models[i].Dispose();
      modelIds.Dispose();
      materialIds.Dispose();
      models.Dispose();
    }
  }
}
