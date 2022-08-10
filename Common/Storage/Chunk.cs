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


    public IBlock this[BlockPos pos]
    {
      get
      {
        var index = pos.z * 16 * 16 + pos.y * 16 + pos.x;
        return this.blocks[index];
      }
    }


    /// <summary>
    /// Creates a new chunk instance.
    /// </summary>
    /// <param name="world">The world this chunk is in.</param>
    /// <param name="pos">The position of this chunk in the world.</param>
    internal Chunk(IWorld world, BlockPos pos)
    {
      World = world;
      Position = pos;

      foreach (var p in CuboidIterator.OverChunk())
      {
        int index = p.z * 16 * 16 + p.y * 16 + p.z;
        this.blocks[index] = new Block(this, pos + p);
      }
    }
  }
}
