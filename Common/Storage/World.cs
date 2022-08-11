using System;
using LibSugar;
using System.Collections.Generic;

namespace Bones3.Storage
{
  public class World : IWorld
  {
    private readonly Dictionary<BlockPos, Chunk> chunks = new Dictionary<BlockPos, Chunk>();
    private IBlockType defaultBlockType;


    /// <summary>
    /// Creates a new World instance.
    /// </summary>
    /// <param name="defaultBlockType">The default block type to use when generating new chunks.</param>
    public World(IBlockType defaultBlockType)
    {
      this.defaultBlockType = defaultBlockType;
    }


    /// <inheritdoc/>
    public Option<IChunk> GetChunk(BlockPos pos, bool create)
    {
      pos &= ~15;

      if (this.chunks.ContainsKey(pos)) return Option<IChunk>.Some(this.chunks[pos]);
      if (!create) return Option<IChunk>.None;

      var chunk = new Chunk(this, pos, this.defaultBlockType);
      this.chunks[pos] = chunk;
      return Option<IChunk>.Some(chunk);
    }


    /// <inheritdoc/>
    public Option<IBlock> GetBlock(BlockPos pos, bool create)
    {
      return GetChunk(pos, create).Map(c => c[pos]);
    }


    /// <inheritdoc/>
    public void Dispose()
    {
      foreach (var chunk in this.chunks.Values)
        chunk.Dispose();

      this.chunks.Clear();
    }
  }
}
