using System;

namespace Bones3
{
  /// <summary>
  /// Represents a 16x16x16 slice of a world containing a grid of voxels.
  /// </summary>
  public interface IChunk : IDisposable
  {
    /// <summary>
    /// Gets the world that this chunk is in.
    /// </summary>
    IWorld World { get; }


    /// <summary>
    /// Gets the block coordinates of this chunk, as defined by the location of
    /// the block within this chunk located at 0, 0, 0 in local coordinates.
    /// </summary>
    BlockPos Position { get; }


    /// <summary>
    /// Gets the block at the given location within this chunk.
    /// </summary>
    /// <param name="pos">The position of the block, in local coordinates.</param>
    /// <returns>The block.</returns>
    /// <exception cref="IndexOutOfRangeException">If the block position is in within this chunk.</example>
    IBlock this[BlockPos pos] { get; }
  }
}
