using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Mathematics;

namespace Bones3
{
  public struct Direction
  {
    /// <summary>
    /// The north-facing directional enum value.
    /// </summary>
    public static readonly Direction North = new Direction(0, new BlockPos(0, 0, -1));


    /// <summary>
    /// The east-facing directional enum value.
    /// </summary>
    public static readonly Direction East = new Direction(1, new BlockPos(1, 0, 0));


    /// <summary>
    /// The south-facing directional enum value.
    /// </summary>
    public static readonly Direction South = new Direction(2, new BlockPos(0, 0, 1));


    /// <summary>
    /// The west-facing directional enum value.
    /// </summary>
    public static readonly Direction West = new Direction(3, new BlockPos(-1, 0, 0));


    /// <summary>
    /// The up-facing directional enum value.
    /// </summary>
    public static readonly Direction Up = new Direction(4, new BlockPos(0, 1, 0));


    /// <summary>
    /// The down-facing directional enum value.
    /// </summary>
    public static readonly Direction Down = new Direction(5, new BlockPos(0, -1, 0));


    /// <summary>
    /// The index value of this direction.
    /// </summary>
    public int Index { get; private set; }


    /// <summary>
    /// Gets the block-position vector version of this direction.
    /// </summary>
    public BlockPos AsBlockPos { get; private set; }


    /// <summary>
    /// Converts this direction into a Unity Vector3.
    /// </summary>
    public Vector3 AsVector3 => AsBlockPos.AsVector3;


    public int3 AsInt3 => new int3(AsBlockPos.x, AsBlockPos.y, AsBlockPos.z);


    /// <summary>
    /// Gets the opposite of this direction.
    /// </summary>
    public Direction Opposite
    {
      get
      {
        switch (Index)
        {
          case 0: return Direction.South;
          case 1: return Direction.West;
          case 2: return Direction.North;
          case 3: return Direction.East;
          case 4: return Direction.Down;
          case 5: return Direction.Up;
          default: return default;
        }
      }
    }


    /// <summary>
    /// Creates a new Direction instance.
    /// </summary>
    /// <param name="index">The index value of this direction.</param>
    /// <param name="asVector">The directional unit vector.</param>
    private Direction(int index, BlockPos asVector)
    {
      Index = index;
      AsBlockPos = asVector;
    }


    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      return obj is Direction dir && dir.Index == this.Index;
    }


    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return this.Index;
    }


    /// <inheritdoc/>
    public override string ToString()
    {
      return this.AsBlockPos.ToString();
    }


    /// <summary>
    /// Checks whether or not the two given directions are equal.
    /// </summary>
    /// <param name="a">The first direction.</param>
    /// <param name="b">The second direction.</param>
    /// <returns>True if the directions are equal, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Direction a, Direction b)
    {
      return a.Index == b.Index;
    }


    /// <summary>
    /// Checks whether or not the two given directions are inequal.
    /// </summary>
    /// <param name="a">The first direction.</param>
    /// <param name="b">The second direction.</param>
    /// <returns>True if the directions are not equal, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Direction a, Direction b)
    {
      return a.Index != b.Index;
    }


    /// <summary>
    /// Gets a direction by it's corresponding index value.
    /// </summary>
    /// <param name="index">The index value of the direction.</param>
    /// <returns>The direction value with the matching index.</returns>
    /// <exception cref="ArgumentException">If the index value does not have a matching direction.</exception>
    public static Direction FromIndex(int index)
    {
      switch (index)
      {
        case 0: return Direction.North;
        case 1: return Direction.East;
        case 2: return Direction.South;
        case 3: return Direction.West;
        case 4: return Direction.Up;
        case 5: return Direction.Down;
        default: throw new ArgumentException("Invalid direction index!", nameof(index));
      }
    }
  }
}
