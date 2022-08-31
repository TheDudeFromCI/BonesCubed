using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Bones3.Util;
using Bones3.Native.Unsafe;

namespace Bones3.Runtime
{
  /// <summary>
  /// A Unity-friendly implementation of a serializable block type. This asset
  /// is intended to be used with Unity Addressables for access and use.
  /// </summary>
  [CreateAssetMenu(fileName = "New Block List", menuName = "Bones3/Block List")]
  public class BlockList : ScriptableObject, IBlockList
  {
    [SerializeField]
    [ReorderableList]
    [Tooltip("A list of all blocks in this list.")]
    private BlockType[] blocks;


    [NonSerialized]
    private List<IBlockModel> modelList;

    [NonSerialized]
    private List<Material> materialList;

    [NonSerialized]
    private List<BlockMeta> metaList;

    [NonSerialized]
    private bool cacheLoaded;


    /// <summary>
    /// Loads the block data cache for indexing purposes.
    /// </summary>
    private void LoadCache()
    {
      if (cacheLoaded) return;
      cacheLoaded = true;

      this.modelList = new List<IBlockModel>();
      this.materialList = new List<Material>();
      this.metaList = new List<BlockMeta>();

      for (int i = 0; i < this.blocks.Length; i++)
      {
        if (!this.modelList.Contains(this.blocks[i].BlockModel)) this.modelList.Add(this.blocks[i].BlockModel);
        if (!this.materialList.Contains(this.blocks[i].Material)) this.materialList.Add(this.blocks[i].Material);

        var meta = new BlockMeta();
        meta.modelId = (ushort)(this.modelList.IndexOf(this.blocks[i].BlockModel) + 1);
        meta.materialId = (ushort)this.materialList.IndexOf(this.blocks[i].Material);
        this.metaList.Add(meta);
      }
    }


    /// <inheritdoc/>
    public JobHandle LoadBlockModels(out NativeArray<UnsafeBlockModel> generatedModels, out Mesh.MeshDataArray meshData)
    {
      LoadCache();
      return MeshUtilities.LoadBlockModels(this.modelList, out generatedModels, out meshData);
    }


    /// <inheritdoc/>
    public Material[] LoadBlockMaterials()
    {
      LoadCache();
      return this.materialList.ToArray();
    }


    /// <inheritdoc/>
    public NativeArray<BlockMeta> LoadBlockMetas()
    {
      LoadCache();
      var array = new NativeArray<BlockMeta>(this.metaList.Count, Allocator.Persistent);
      for (int i = 0; i < array.Length; i++) array[i] = this.metaList[i];
      return array;
    }


    /// <inheritdoc/>
    public int GetBlockID(string name)
    {
      for (int i = 0; i < this.blocks.Length; i++)
        if (this.blocks[i].Name.Equals(name)) return i;

      return -1;
    }
  }
}
