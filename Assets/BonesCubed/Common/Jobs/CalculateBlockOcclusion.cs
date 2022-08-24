using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Bones3.Native;
using Bones3.Native.Unsafe;

namespace Bones3.Jobs
{
  [BurstCompile]
  public struct CalculateBlockOcclusion : IJobParallelFor
  {
    /// <summary>
    /// The list of block models to read the mesh data from.
    /// </summary>
    [ReadOnly]
    public NativeArray<UnsafeBlockModel> models;


    /// <summary>
    /// The model ids of the world that contains the target region.
    /// </summary>
    [ReadOnly]
    public NativeInfiniteGrid3D<ushort> modelIds;


    /// <summary>
    /// The output occlusion and visiblity data for each block within a region
    /// of the infinite modelIds grid.
    /// </summary>
    [WriteOnly]
    public NativeGrid3D<IBlockModel.OccludingSegment>.Concurrent blockVisiblity;


    /// <inheritdoc/>
    [BurstCompile]
    public void Execute(int index)
    {
      var pos = this.blockVisiblity.Region.PositionFromIndex(index);
      if (ModelIndexAt(pos) < 0) this.blockVisiblity.SetElement(index, IBlockModel.OccludingSegment.None);
      else this.blockVisiblity.SetElement(index, CalculateVisibleSegments(pos));
    }


    /// <summary>
    /// Calculates the visible block segments for the given block position based
    /// off of the occlusion values of the neighboring blocks.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <returns>The segments of this block that are visible.</returns>
    [BurstCompile]
    private IBlockModel.OccludingSegment CalculateVisibleSegments(int3 pos)
    {
      var segment = IBlockModel.OccludingSegment.None;
      if (ModelIndexAt(pos) < 0) return segment;

      segment |= IsFaceHidden(pos, Direction.North) ? 0 : OccludingSegmentFromDirection(Direction.North);
      segment |= IsFaceHidden(pos, Direction.East) ? 0 : OccludingSegmentFromDirection(Direction.East);
      segment |= IsFaceHidden(pos, Direction.South) ? 0 : OccludingSegmentFromDirection(Direction.South);
      segment |= IsFaceHidden(pos, Direction.West) ? 0 : OccludingSegmentFromDirection(Direction.West);
      segment |= IsFaceHidden(pos, Direction.Up) ? 0 : OccludingSegmentFromDirection(Direction.Up);
      segment |= IsFaceHidden(pos, Direction.Down) ? 0 : OccludingSegmentFromDirection(Direction.Down);

      if (segment > 0) segment |= IBlockModel.OccludingSegment.Center;
      return segment;
    }


    /// <summary>
    /// Converts the given direction value to an occluding segment flag.
    /// </summary>
    /// <param name="face">The direction value.</param>
    /// <returns>The occluding segment flag.</returns>
    [BurstCompile]
    private IBlockModel.OccludingSegment OccludingSegmentFromDirection(Direction face)
    {
      if (face == Direction.North) return IBlockModel.OccludingSegment.North;
      if (face == Direction.East) return IBlockModel.OccludingSegment.East;
      if (face == Direction.South) return IBlockModel.OccludingSegment.South;
      if (face == Direction.West) return IBlockModel.OccludingSegment.West;
      if (face == Direction.Up) return IBlockModel.OccludingSegment.Top;
      if (face == Direction.Down) return IBlockModel.OccludingSegment.Bottom;
      return IBlockModel.OccludingSegment.Center;
    }


    /// <summary>
    /// Checks if the given face of a block is occluded or not.
    /// </summary>
    /// <param name="pos">The position of the block to check.</param>
    /// <param name="face">The direction to check.</param>
    /// <returns>True if the face is hidden, false otherwise.</returns>
    [BurstCompile]
    private bool IsFaceHidden(int3 pos, Direction face)
    {
      pos += face.AsInt3;
      var segment = OccludingSegmentFromDirection(face.Opposite);

      int modelIndex = ModelIndexAt(pos);
      if (modelIndex < 0) return false;

      var model = this.models[modelIndex];
      return (model.OccludingSegments & segment) > 0;
    }


    /// <summary>
    /// Gets the model index at the given block position.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <returns>The model index, or -1 if the block is empty.</returns>
    [BurstCompile]
    private int ModelIndexAt(int3 pos)
    {
      return this.modelIds.GetElement(pos) - 1;
    }
  }
}
