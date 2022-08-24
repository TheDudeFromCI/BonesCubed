using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace Bones3
{
  public struct Region
  {
    /// <summary>
    /// The minimum corner position of this region.
    /// </summary>
    public int3 position;


    /// <summary>
    /// The size of this region.
    /// </summary>
    public int3 size;


    /// <summary>
    /// Gets the total number of elements within this region.
    /// </summary>
    public int Length => this.size.x * this.size.y * this.size.z;


    /// <summary>
    /// Creates a new region instance.
    /// </summary>
    /// <param name="position">The position of this region.</param>
    /// <param name="size">The size of this region.</param>
    public Region(int3 position, int3 size)
    {
      this.position = position;
      this.size = size;
    }


    /// <summary>
    /// Maps the given position within this region to a unquie index value.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <returns>The index value.</returns>
    /// <exception cref="IndexOutOfRangeException">If the position is outside of this region.</exception>
    public int IndexFromPosition(int3 pos)
    {
      if (!Contains(pos)) throw new IndexOutOfRangeException($"Position {pos} is not inside region {this}");

      pos -= this.position;
      return pos.z * this.size.x * this.size.y + pos.y * this.size.x + pos.x;
    }


    /// <summary>
    /// Maps the given index to a unquie position within this grid.
    /// </summary>
    /// <param name="index">The index to convert.</param>
    /// <returns>The position.</returns>
    /// <exception cref="IndexOutOfRangeException">If the index does not exist within this region.</exception>
    public int3 PositionFromIndex(int index)
    {
      if (index < 0 || index >= Length) throw new IndexOutOfRangeException($"Index {index} is invalid for container of length {Length}");

      int z = index / (size.x * size.y);
      int y = (index / size.x) % size.y;
      int x = index % size.x;
      return new int3(x, y, z) + this.position;
    }


    /// <summary>
    /// Gets whether or not the given position is inside this grid bounds.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <returns>True if the position is inside this grid. False otherwise.</returns>
    public bool Contains(int3 pos)
    {
      pos -= this.position;
      if (pos.x < 0 || pos.y < 0 || pos.z < 0) return false;
      return pos.x < this.size.x && pos.y < this.size.y && pos.z < this.size.z;
    }


    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      return obj is Region r && math.all(r.size == this.size & r.position == this.position);
    }


    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return HashCode.Combine(this.size, this.position);
    }


    /// <inheritdoc/>
    public override string ToString()
    {
      return $"Region(Position: {this.position}; Size: {this.size})";
    }


    /// <summary>
    /// Checks if two regions are equal.
    /// </summary>
    /// <param name="a">The first region.</param>
    /// <param name="b">The second region.</param>
    /// <returns>True if both regions are equal. False otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Region a, Region b)
    {
      return math.all(a.position == b.position & a.size == b.size);
    }


    /// <summary>
    /// Checks if two regions are not equal.
    /// </summary>
    /// <param name="a">The first region.</param>
    /// <param name="b">The second region.</param>
    /// <returns>True if both regions are not equal. False otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Region a, Region b)
    {
      return !math.all(a.position == b.position & a.size == b.size);
    }
  }
}
