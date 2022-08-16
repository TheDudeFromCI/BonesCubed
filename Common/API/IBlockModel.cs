using UnityEngine;
using System.Collections.Generic;

namespace Bones3
{
  /// <summary>
  /// Contains data required to construct this model variant and apply it to a
  /// chunk mesh.
  /// </summary>
  public struct ModelVariant
  {
    /// <summary>
    /// The transformation matrix to apply to the mesh model variant before
    /// applying it.
    /// </summary>
    public Matrix4x4 transform;


    /// <summary>
    /// Gets the index of the model submesh within the global mesh data array.
    /// </summary>
    public int meshIndex;
  }


  /// <summary>
  /// A collection of possible block models that may be applied to a given block
  /// for a given block type. The variant that is used is based of a set of
  /// conditions that must be met.
  /// </summary>
  public interface IBlockModel
  {
    /// <summary>
    /// Gets a list of all variants for this block model. This method also
    /// serves to calculate the global mesh list to use for generating the
    /// reference data to pass to the Unity Job System.
    /// </summary>
    /// <param name="meshList">A list of meshes to use for calculating the mesh index within the variants.</param>
    /// <returns>A list of all variants.</returns>
    List<ModelVariant> GetVariants(List<Mesh> meshList);
  }
}
