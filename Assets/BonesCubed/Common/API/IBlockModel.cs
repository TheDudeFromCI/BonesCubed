using System;
using UnityEngine;

namespace Bones3
{
  /// <summary>
  /// A model object that can be applied to a block in order to define how it
  /// should be rendered within the game.
  /// </summary>
  public interface IBlockModel
  {
    /// <summary>
    /// An enum to indicate which segement of an occluding block model a vertex
    /// belongs to.
    /// </summary>
    [Flags]
    public enum OccludingSegment
    {
      None = 0,
      North = 1,
      East = 2,
      South = 4,
      West = 8,
      Top = 16,
      Bottom = 32,
      Center = 64,
      Everything = 127,
    }


    /// <summary>
    /// Gets the mesh object for this model that should be baked into the chunk
    /// mesh.
    /// </summary>
    Mesh Mesh { get; }


    /// <summary>
    /// Gets the directions of this block model that block neighboring blocks.
    /// </summary>
    OccludingSegment OccludingDirections { get; }
  }
}
