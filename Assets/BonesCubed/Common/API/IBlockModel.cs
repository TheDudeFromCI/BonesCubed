using UnityEngine;
using System.Collections.Generic;

namespace Bones3
{
  /// <summary>
  /// A model object that can be applied to a block in order to define how it
  /// should be rendered within the game.
  /// </summary>
  public interface IBlockModel
  {
    /// <summary>
    /// Gets the mesh object for this model that should be baked into the chunk
    /// mesh.
    /// </summary>
    Mesh StaticMesh { get; }


    /// <summary>
    /// Gets the directions of this block model that block neighboring blocks.
    /// </summary>
    OccludingVoxelVertexSegement OccludingDirections { get; }
  }
}
