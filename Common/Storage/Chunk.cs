using System;
using Bones3.Iterators;

namespace Bones3.Storage
{
  internal class Chunk : IChunk
  {
    private readonly IBlock[] blocks = new IBlock[16 * 16 * 16];


    /// <inheritdoc/>
    public IWorld World { get; }


    /// <inheritdoc/>
    public BlockPos Position { get; }


    /// <inheritdoc/>
    public IBlock this[BlockPos pos]
    {
      get
      {
        if (pos < new BlockPos(0, 0, 0) || pos >= new BlockPos(16, 16, 16))
          throw new IndexOutOfRangeException($"Block position {pos} is outside of chunk bounds!");

        var index = pos.z * 16 * 16 + pos.y * 16 + pos.x;
        return this.blocks[index];
      }
    }


    /// <summary>
    /// Creates a new chunk instance.
    /// </summary>
    /// <param name="world">The world this chunk is in.</param>
    /// <param name="pos">The position of this chunk in the world.</param>
    /// <param name="blockType">The default block type to fill this chunk with.</param>
    internal Chunk(IWorld world, BlockPos pos, IBlockType blockType)
    {
      World = world;
      Position = pos;

      foreach (var p in CuboidIterator.OverChunk())
      {
        int index = p.z * 16 * 16 + p.y * 16 + p.z;
        this.blocks[index] = new Block(this, pos + p, blockType);
      }
    }
  }
}
