using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Bones3.Native.Unsafe;

namespace Bones3
{
  /// <summary>
  /// Defines a list of block types.
  /// </summary>
  public interface IBlockList
  {
    /// <summary>
    /// This function creates a job handle for converting this block list into
    /// a list of unmanaged block model data for use in mesh generation jobs.
    /// </summary>
    /// <param name="generatedModels">The array of generated models, populated after the job completes.</param>
    /// <param name="meshData">The temporary mesh data being read from. Must be manually disposed on job completion.</param>
    /// <returns>The job handle for the block loading task.</returns>
    JobHandle LoadBlockModels(out NativeArray<UnsafeBlockModel> generatedModels, out Mesh.MeshDataArray meshData);


    /// <summary>
    /// Gets an array of all block materials. The index of each material in the
    /// array is the material id for that material.
    /// </summary>00
    /// <returns>A list of materials for this block list.</returns>
    Material[] LoadBlockMaterials();


    /// <summary>
    /// Gets a persistant native array of all block metas stored in this block
    /// list.
    /// </summary>
    /// <returns>A list of block metas.</returns>
    NativeArray<BlockMeta> LoadBlockMetas();


    /// <summary>
    /// Gets the ID of the block within this block list with the given name.
    /// </summary>
    /// <param name="name">The name of the block.</param>
    /// <returns>The block ID, or -1 if the block was not found.</returns>
    int GetBlockID(string name);
  }
}
