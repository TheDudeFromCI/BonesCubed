using System;
using LibSugar;

namespace Bones3
{
  /// <summary>
  /// An infinite grid of blocks stored within chunks.
  /// </summary>
  public interface IWorld : IDisposable
  {
    /// <summary>
    /// Gets the chunk that contains the given block coordinate.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="create">Whether or not to create the chunk if it does not currently exist.</param>
    /// <returns>The chunk, or null if it does not exist or could not be created.</returns>
    Option<IChunk> GetChunk(BlockPos pos, bool create);


    /// <summary>
    /// Gets the block at the specified block coordinate.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="create">Whether or not to create the block if it does not currently exist.</param>
    /// <returns>The block, or null if it does not exist or could not be created.</returns>
    Option<IBlock> GetBlock(BlockPos pos, bool create);
  }
}
