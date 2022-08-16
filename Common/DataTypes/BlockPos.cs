using System;
using UnityEngine;

namespace Bones3
{
  /// <summary>
  /// Represents a block position in world coordinates.
  /// </summary>
  public struct BlockPos
  {
    public int x;
    public int y;
    public int z;


    /// <summary>
    /// Converts this block position into a Unity Vector3.
    /// </summary>
    public Vector3 AsVector3 => new Vector3(x, y, z);


    /// <summary>
    /// Gets the local chunk index for this block position.
    /// </summary>
    public int Index => (z & 15) * 16 * 16 + (y & 15) * 16 + (x & 15);


    /// <summary>
    /// Creates a new block posiiton instance at the given coordinates.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    public BlockPos(int x, int y, int z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
    }


    /// <summary>
    /// Offsets this block position by a given direction and number of units in
    /// that direction. The result is then returned as a new block position.
    /// </summary>
    /// <param name="direction">The direction of the offset.</param>
    /// <param name="units">The number of units to shift.</param>
    /// <returns>The new block position.</returns>
    public BlockPos Offset(Direction direction, int units)
    {
      return this + direction.AsBlockPos * units;
    }


    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      return obj is BlockPos pos &&
             x == pos.x &&
             y == pos.y &&
             z == pos.z;
    }


    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return HashCode.Combine(x, y, z);
    }


    /// <inheritdoc/>
    public override string ToString()
    {
      return $"({x}, {y}, {z})";
    }


    /// <summary>
    /// Creates a new block position using the smallest value from each
    /// coordinate axis.
    /// </summary>
    /// <param name="a">The first block position.</param>
    /// <param name="b">The second block position.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos Min(BlockPos a, BlockPos b)
    {
      return new BlockPos(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
    }


    /// <summary>
    /// Creates a new block position using the largest value from each
    /// coordinate axis.
    /// </summary>
    /// <param name="a">The first block position.</param>
    /// <param name="b">The second block position.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos Max(BlockPos a, BlockPos b)
    {
      return new BlockPos(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
    }


    /// <summary>
    /// Applies the '&' bitwise operator to each axis coordinate.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="val">The mask value.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos operator &(BlockPos pos, int val)
    {
      return new BlockPos(pos.x & val, pos.y & val, pos.z & val);
    }


    /// <summary>
    /// Applies the '>>' bitwise operator to each axis coordinate.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="val">The bit shift value.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos operator >>(BlockPos pos, int val)
    {
      return new BlockPos(pos.x >> val, pos.y >> val, pos.z >> val);
    }


    /// <summary>
    /// Applies the '<<' bitwise operator to each axis coordinate.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="val">The bit shift value.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos operator <<(BlockPos pos, int val)
    {
      return new BlockPos(pos.x << val, pos.y << val, pos.z << val);
    }


    /// <summary>
    /// Adds together each axis coordinate from two block positions.
    /// </summary>
    /// <param name="pos">The first block position.</param>
    /// <param name="val">The second block position.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos operator +(BlockPos a, BlockPos b)
    {
      return new BlockPos(a.x + b.x, a.y + b.y, a.z + b.z);
    }


    /// <summary>
    /// Scales the block position vector by a given integer value.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="val">The scaler value.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos operator *(BlockPos pos, int val)
    {
      return new BlockPos(pos.x * val, pos.y * val, pos.z * val);
    }


    /// <summary>
    /// Shifts the block position vector by a given integer value along all
    /// axises.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="val">The scaler value.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos operator +(BlockPos pos, int val)
    {
      return new BlockPos(pos.x + val, pos.y + val, pos.z + val);
    }


    /// <summary>
    /// Shifts the block position vector by one unit in a given direction.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="val">The direction.</param>
    /// <returns>The new block position.</returns>
    public static BlockPos operator +(BlockPos pos, Direction dir)
    {
      return pos + dir.AsBlockPos;
    }


    /// <summary>
    /// Checks if two block positions are equal.
    /// </summary>
    /// <param name="a">The first block position.</param>
    /// <param name="b">The second block position.</param>
    /// <returns>True if both block positions are equal. False otherwise.</returns>
    public static bool operator ==(BlockPos a, BlockPos b)
    {
      return a.x == b.x && a.y == b.y && a.z == b.z;
    }


    /// <summary>
    /// Checks if two block positions are not equal.
    /// </summary>
    /// <param name="a">The first block position.</param>
    /// <param name="b">The second block position.</param>
    /// <returns>True if both block positions are not equal. False otherwise.</returns>
    public static bool operator !=(BlockPos a, BlockPos b)
    {
      return a.x != b.x || a.y != b.y || a.z != b.z;
    }


    /// <summary>
    /// Checks if a block position is smaller along all axises than another
    /// block position.
    /// </summary>
    /// <param name="a">The first block position.</param>
    /// <param name="b">The second block position.</param>
    /// <returns>True if the first block position is smaller than the second. False otherwise.</returns>
    public static bool operator <(BlockPos a, BlockPos b)
    {
      return a.x < b.x && a.y < b.y && a.z < b.z;
    }


    /// <summary>
    /// Checks if a block position is greater along all axises than another
    /// block position.
    /// </summary>
    /// <param name="a">The first block position.</param>
    /// <param name="b">The second block position.</param>
    /// <returns>True if the first block position is greater than the second. False otherwise.</returns>
    public static bool operator >(BlockPos a, BlockPos b)
    {
      return a.x > b.x && a.y > b.y && a.z > b.z;
    }


    /// <summary>
    /// Checks if a block position is smaller, or equal to, all axises of
    /// another block position.
    /// </summary>
    /// <param name="a">The first block position.</param>
    /// <param name="b">The second block position.</param>
    /// <returns>True if the first block position is smaller, or equal to, the second. False otherwise.</returns>
    public static bool operator <=(BlockPos a, BlockPos b)
    {
      return a.x <= b.x && a.y <= b.y && a.z <= b.z;
    }


    /// <summary>
    /// Checks if a block position is greater, or equal to, all axises of
    /// another block position.
    /// </summary>
    /// <param name="a">The first block position.</param>
    /// <param name="b">The second block position.</param>
    /// <returns>True if the first block position is greater, or equal to, the second. False otherwise.</returns>
    public static bool operator >=(BlockPos a, BlockPos b)
    {
      return a.x >= b.x && a.y >= b.y && a.z >= b.z;
    }
  }
}
