using System;
using System.Collections;
using Bones3.Iterators;
using Bones3.Native;
using Unity.Collections;

namespace Bones3.Storage
{
  internal class Chunk : IChunk
  {
    private readonly IBlock[] blocks = new IBlock[16 * 16 * 16];
    private readonly Hashtable fields = new Hashtable();


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

      fields.Add("mesh_index", new NativeGrid3D<ushort>(new BlockPos(16, 16, 16), Allocator.Persistent));
    }


    /// <inheritdoc/>
    public void Dispose()
    {
      foreach (var field in this.fields.Values)
        ((IDisposable)field).Dispose();
    }


    /// <summary>
    /// Gets the contents of a field within this chunk. This value should not be
    /// edited directly and is only intended for read purposes when passing to
    /// the Unity job system.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the field.</typeparam>
    /// <param name="name">The name of the field.</param>
    /// <returns>The NativeGrid3D data containing the field contents.</returns>
    /// <exception cref="ArgumentException">If the field does not exist.</exception>
    public NativeGrid3D<T> GetField<T>(string name) where T : struct
    {
      if (!this.fields.ContainsKey(name)) throw new ArgumentException("Field does not exist!", nameof(name));
      return (NativeGrid3D<T>)this.fields[name];
    }
  }
}
