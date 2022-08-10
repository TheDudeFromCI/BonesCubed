using System.Collections.Generic;

namespace Bones3.Iterators
{
  /// <summary>
  /// A cuboid shaped block position iterator.
  /// </summary>
  public class CuboidIterator : IEnumerable<BlockPos>
  {
    /// <summary>
    /// Creates a new CuboidIterator instance over all blocks in a single chunk
    /// locatated at the world origin.
    /// </summary>
    /// <returns>A new cuboid interator instance.</returns>
    public static CuboidIterator OverChunk()
    {
      var min = new BlockPos(0, 0, 0);
      var max = new BlockPos(15, 15, 15);
      return new CuboidIterator(min, max);
    }

    /// <summary>
    /// Creates a new CuboidIterator instance from two opposite positions that
    /// define the selected region boundries.
    /// </summary>
    /// <param name="p1">The first position.</param>
    /// <param name="p2">The second position.</param>
    /// <returns>A new cuboid iterator instance.</returns>
    public static CuboidIterator FromTwoPoints(BlockPos p1, BlockPos p2)
    {
      var min = BlockPos.Min(p1, p2);
      var max = BlockPos.Max(p1, p2);
      return new CuboidIterator(min, max);
    }

    private readonly BlockPos min;
    private readonly BlockPos max;


    /// <summary>
    /// Creates a new CuboidIterator instance from min and max coordinates.
    /// </summary>
    /// <param name="min">The minimum block coordinates.</param>
    /// <param name="max">The maximum block coordinates.</param>
    private CuboidIterator(BlockPos min, BlockPos max)
    {
      this.min = min;
      this.max = max;
    }


    /// <inheritdoc/>
    public IEnumerator<BlockPos> GetEnumerator()
    {
      for (int x = min.x; x <= max.x; x++)
      {
        for (int z = min.z; z <= max.z; z++)
        {
          for (int y = min.y; y <= max.y; y++)
          {
            yield return new BlockPos(x, y, z);
          }
        }
      }
    }


    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
