using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bones3
{
  public sealed class Direction
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
    /// A read-only list of all direction values in the order of: North, East,
    /// South, West, Up, Down.
    /// </summary>
    public static readonly IList<Direction> All = Array.AsReadOnly(new Direction[] { North, East, South, West, Up, Down });


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
          default: return null;
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
