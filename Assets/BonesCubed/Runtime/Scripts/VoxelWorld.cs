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
    [SerializeField]
    [Tooltip("The block list to use for this world.")]
    private BlockList blockList;


    private NativeInfiniteGrid3D<ushort> modelIDGrid;
    private NativeInfiniteGrid3D<ushort> materialIDGrid;

    private NativeArray<UnsafeBlockModel> models;
    private NativeArray<BlockMeta> metas;
    private Material[] materials;


    void Awake()
    {
      Mesh.MeshDataArray meshDataArray;
      blockList.LoadBlockModels(out this.models, out meshDataArray).Complete();
      meshDataArray.Dispose();

      this.materials = blockList.LoadBlockMaterials();
      this.metas = blockList.LoadBlockMetas();

      this.modelIDGrid = new NativeInfiniteGrid3D<ushort>(Allocator.Persistent);
      this.materialIDGrid = new NativeInfiniteGrid3D<ushort>(Allocator.Persistent);
    }


    void Start()
    {
      SetBlock(new int3(0, 0, 0), "Grass");
      SetBlock(new int3(0, 0, 1), "Grass");
      GenerateMesh(new Region(new int3(-5, -5, -5), new int3(11, 11, 11)));
    }


    void OnDestroy()
    {
      this.modelIDGrid.Dispose();
      this.materialIDGrid.Dispose();

      for (int i = 0; i < models.Length; i++) models[i].Dispose();
      this.models.Dispose();
      this.metas.Dispose();
    }


    public void SetBlock(int3 pos, string name)
    {
      var blockId = this.blockList.GetBlockID(name);
      if (blockId < 0) throw new System.ArgumentException("Block not found!", nameof(name));

      this.modelIDGrid.SetElement(pos, this.metas[blockId].modelId);
      this.materialIDGrid.SetElement(pos, this.metas[blockId].materialId);
    }


    public void GenerateMesh(Region region)
    {
      Mesh.MeshDataArray meshData;
      NativeList<int> materialIndices;
      MeshUtilities.RemeshRegion(region, this.modelIDGrid, this.materialIDGrid, this.models, out meshData, out materialIndices).Complete();

      var mesh = new Mesh();
      Mesh.ApplyAndDisposeWritableMeshData(meshData, mesh);

      var sharedMaterials = new Material[materialIndices.Length];
      for (int i = 0; i < sharedMaterials.Length; i++) sharedMaterials[i] = this.materials[materialIndices[i]];
      materialIndices.Dispose();

      var go = new GameObject();
      go.AddComponent<MeshFilter>().sharedMesh = mesh;
      go.AddComponent<MeshRenderer>().sharedMaterials = sharedMaterials;
    }
  }
}
