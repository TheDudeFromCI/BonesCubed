using System;
using System.Collections;
using Unity.Collections;
using Bones3.Native;

namespace Bones3
{
  /// <summary>
  /// Represents a 16x16x16 slice of a world containing a grid of voxels.
  /// </summary>
  public class Chunk : IDisposable
  {
    private readonly Hashtable fields = new Hashtable();


    /// <summary>
    /// Gets the world that this chunk is in.
    /// </summary>
    public World World { get; }


    /// <summary>
    /// Gets the block coordinates of this chunk, as defined by the location of
    /// the block within this chunk located at 0, 0, 0 in local coordinates.
    /// </summary>
    public BlockPos Position { get; }


    /// <summary>
    /// Creates a new chunk instance.
    /// </summary>
    /// <param name="world">The world this chunk is in.</param>
    /// <param name="pos">The position of this chunk in the world.</param>
    internal Chunk(World world, BlockPos pos)
    {
      World = world;
      Position = pos;
    }


    /// <summary>
    /// Disposes and clears all fields within this chunk.
    /// </summary>
    public void Dispose()
    {
      foreach (var field in this.fields.Values)
        ((IDisposable)field).Dispose();

      this.fields.Clear();
    }


    /// <summary>
    /// Gets the native field data within this chunk with the given name.
    /// </summary>
    /// <typeparam name="T">The field data type.</typeparam>
    /// <param name="name">The name of the field.</param>
    /// <returns>The field native array data.</returns>
    /// <exception cref="ArgumentException">If the field does not exist.</exception>
    public NativeGrid3D<T> GetField<T>(string name) where T : struct
    {
      if (!this.fields.ContainsKey(name)) throw new ArgumentException("Field does not exist!", nameof(name));
      return (NativeGrid3D<T>)this.fields[name];
    }


    /// <summary>
    /// Creates a new field within this chunk.
    /// </summary>
    /// <typeparam name="T">The type of data within this field.</typeparam>
    /// <param name="name">The name of this field.</param>
    public void AddField<T>(string name) where T : struct
    {
      var field = new NativeGrid3D<T>(new BlockPos(16, 16, 16), Allocator.Persistent);
      this.fields[name] = field;
    }
  }
}
