using System;
using UnityEngine;
using Unity.Collections;
using Bones3.Native;
using Bones3.Util;

namespace Bones3
{
  /// <summary>
  /// A container for storing asset references that are used by model generation
  /// within the Bone3 pipeline.
  /// </summary>
  public class Bones3AssetReference : IDisposable
  {
    private readonly NativeMesh<OccludingVoxelVertex, ushort> modelAtlas;
    private readonly NativeList<OccludingBlockModel> modelPointers;


    /// <summary>
    /// Gets the global model atlas asset. This data should not be modified from
    /// output of this class.
    /// </summary>
    public NativeMesh<OccludingVoxelVertex, ushort> ModelAtlas => this.modelAtlas;


    /// <summary>
    /// Gets the global model pointers asset.  This data should not be modified
    /// from output of this class.
    /// </summary>
    public NativeList<OccludingBlockModel> ModelPointers => this.modelPointers;


    /// <summary>
    /// Creates a new Bones3AssetReference instance.
    /// </summary>
    public Bones3AssetReference()
    {
      this.modelAtlas = new NativeMesh<OccludingVoxelVertex, ushort>(Allocator.Persistent);
      this.modelPointers = new NativeList<OccludingBlockModel>(64, Allocator.Persistent);
    }


    /// <summary>
    /// Loads a block model into this asset database.
    /// </summary>
    /// <param name="blockModel">The block model to load.</param>
    /// <returns>The model index.</returns>
    public ushort LoadBlockModel(IBlockModel blockModel)
    {
      var model = MeshUtilities.BakeBlockModelIntoAtlas(blockModel, this.modelAtlas, 0, true);
      this.modelPointers.Add(model);
      return (ushort)this.modelPointers.Length;
    }


    /// <summary>
    /// Disposes all internal asset references.
    /// </summary>
    public void Dispose()
    {
      this.modelAtlas.Dispose();
      this.modelPointers.Dispose();
    }
  }
}
