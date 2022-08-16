using System;
using LibSugar;
using System.Collections.Generic;

namespace Bones3
{
  /// <summary>
  /// An infinite grid of blocks stored within chunks.
  /// </summary>
  public abstract class World : IDisposable
  {
    private readonly Dictionary<BlockPos, Chunk> chunks = new Dictionary<BlockPos, Chunk>();


    /// <summary>
    /// Gets the chunk that contains the given block coordinate.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="create">Whether or not to create the chunk if it does not currently exist.</param>
    /// <returns>The chunk, or null if it does not exist or could not be created.</returns>
    public Option<Chunk> GetChunk(BlockPos pos, bool create)
    {
      pos &= ~15;

      if (this.chunks.ContainsKey(pos)) return Option<Chunk>.Some(this.chunks[pos]);
      if (!create) return Option<Chunk>.None;

      var chunk = new Chunk(this, pos);
      this.chunks[pos] = chunk;
      CreateFields(chunk);

      return Option<Chunk>.Some(chunk);
    }


    /// <summary>
    /// Disposes and clears all chunks within this world.
    /// </summary>
    public void Dispose()
    {
      foreach (var chunk in this.chunks.Values)
        chunk.Dispose();

      this.chunks.Clear();
    }


    /// <summary>
    /// Creates all fields that should exist within this chunk. This is called
    /// once per chunk when that chunk is created or loaded. If a chunk is
    /// unloaded and then loaded again later, this method is called for the new
    /// chunk instance.
    /// </summary>
    /// <param name="chunk">The new chunk.</param>
    protected abstract void CreateFields(Chunk chunk);
  }
}
