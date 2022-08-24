using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Bones3.Native.Unsafe
{
  /// <summary>
  /// Represents a raw block model stored in unmanaged memory.
  /// </summary>
  public unsafe struct UnsafeBlockModel : IDataContainer
  {
    /// <summary>
    /// A collection of vertices and indices that make up a single section of a
    /// block model.
    /// </summary>
    private struct Segment : IDisposable
    {
      internal Allocator allocator;
      internal UnsafeList<VoxelVertex> vertices;
      internal UnsafeList<ushort> indices;
      internal IBlockModel.OccludingSegment modelSegment;


      /// <summary>
      /// Creates a new block model segment.
      /// </summary>
      /// <param name="vertexCount">The number of vertices in this segment.</param>
      /// <param name="indexCount">The number of indices in this segment.</param>
      /// <param name="modelSegment">The occlusion for this segment.</param>
      /// <param name="allocator">The allocator to use for memory management.</param>
      public Segment(int vertexCount, int indexCount, IBlockModel.OccludingSegment modelSegment, Allocator allocator)
      {
        this.allocator = allocator;
        this.vertices = new UnsafeList<VoxelVertex>(vertexCount, allocator);
        this.indices = new UnsafeList<ushort>(indexCount, allocator);
        this.modelSegment = modelSegment;
      }


      /// <inheritdoc/>
      public void Dispose()
      {
        this.vertices.Dispose();
        this.indices.Dispose();
      }
    }


    private Allocator allocator;
    private UnsafeList<Segment> segments;
    private IBlockModel.OccludingSegment occludingSegments;


    /// <inheritdoc/>
    public bool IsCreated => this.segments.IsCreated;

    public IBlockModel.OccludingSegment OccludingSegments => this.occludingSegments;


    /// <summary>
    /// Creates a new UnsafeBlockModel.
    /// </summary>
    /// <param name="occludingSegments">The parts of this model that occlude other block models.</param>
    /// <param name="allocator">The allocator to use for memory management.</param>
    public UnsafeBlockModel(IBlockModel.OccludingSegment occludingSegments, Allocator allocator)
    {
      this.allocator = allocator;
      this.segments = new UnsafeList<Segment>(7, allocator);
      this.occludingSegments = occludingSegments;
    }


    /// <summary>
    /// Adds a new segment to this block model.
    /// </summary>
    /// <param name="vertices">The vertices array to copy from.</param>
    /// <param name="indices">The indices array to copy from.</param>
    /// <param name="modelSegment">The segment of this model, for occlusion handling.</param>
    public void AddSegment(NativeArray<VoxelVertex> vertices, NativeArray<ushort> indices, IBlockModel.OccludingSegment modelSegment)
    {
      var segment = new Segment(vertices.Length, indices.Length, modelSegment, this.allocator);
      segment.vertices.AddRangeNoResize(vertices.GetUnsafePtr<VoxelVertex>(), vertices.Length);
      segment.indices.AddRangeNoResize(indices.GetUnsafePtr<ushort>(), indices.Length);
      this.segments.Add(segment);
    }


    /// <summary>
    /// Gets all vertices and indices within the given segment.
    /// </summary>
    /// <param name="vertices">The vertices list to write to.</param>
    /// <param name="indices">The indices list to write to.</param>
    /// <param name="modelSegment">The segment of this model to retrieve.</param>
    public void GetSegment(ref NativeList<VoxelVertex> vertices, ref NativeList<ushort> indices, IBlockModel.OccludingSegment modelSegment)
    {
      for (int i = 0; i < this.segments.Length; i++)
      {
        if (this.segments[i].modelSegment != modelSegment) continue;

        vertices.AddRange(this.segments[i].vertices.Ptr, this.segments[i].vertices.Length);
        indices.AddRange(this.segments[i].indices.Ptr, this.segments[i].indices.Length);
        return;
      }
    }


    /// <inheritdoc/>
    public void Dispose()
    {
      for (int i = 0; i < this.segments.Length; i++) this.segments[i].Dispose();
      this.segments.Dispose();
    }
  }
}
