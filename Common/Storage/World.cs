using LibSugar;
using System.Collections.Generic;

namespace Bones3.Storage
{
  public class World : IWorld
  {
    private readonly Dictionary<BlockPos, Chunk> chunks = new Dictionary<BlockPos, Chunk>();


    /// <inheritdoc/>
    public Option<IChunk> GetChunk(BlockPos pos, bool create)
    {
      pos &= ~15;

      if (this.chunks.ContainsKey(pos)) return Option<IChunk>.Some(this.chunks[pos]);
      if (!create) return Option<IChunk>.None;

      var chunk = new Chunk(this, pos);
      this.chunks[pos] = chunk;
      return Option<IChunk>.Some(chunk);
    }


    public Option<IBlock> GetBlock(BlockPos pos, bool create)
    {
      return GetChunk(pos, create).Map(c => c[pos]);
    }
  }
}
