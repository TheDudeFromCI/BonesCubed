using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Bones3.Native;

namespace Bones3.Jobs
{
  [BurstCompile]
  public struct GenerateChunkMesh : IJob
  {
    /// <summary>
    /// A 3x3x3 grid of chunks that contain block model pointers for determining
    /// which models to use for each block when generating the mesh.
    /// </summary>
    [ReadOnly]
    public SurroundingChunkGrid<ushort> chunkData;


    /// <summary>
    /// A global atlas of model pointers. These point to the vertex data
    /// locations within the mesh atlas, indicating where mesh data can be
    /// retrieved.
    /// </summary>
    [ReadOnly]
    public NativeList<OccludingBlockModel> modelPointers;


    /// <summary>
    /// The global mesh atlas containing all block models. This mesh is read
    /// from during chunk generation in order to copy mesh data for each block
    /// as needed.
    /// </summary>
    [ReadOnly]
    public NativeMesh<OccludingVoxelVertex, ushort> blockModelAtlas;


    /// <summary>
    /// The native mesh data to write the generated chunk mesh information to.
    /// </summary>
    public NativeMesh<VoxelVertex, ushort> chunkMesh;


    /// <inheritdoc/>
    [BurstCompile]
    public void Execute()
    {
      for (int x = 0; x < 16; x++)
      {
        for (int y = 0; y < 16; y++)
        {
          for (int z = 0; z < 16; z++)
          {
            var pos = new BlockPos(x, y, z);
            var modelIndex = this.chunkData.GetLocalBlock(pos);
            if (modelIndex == 0) continue;
            modelIndex--;

            var modelPointer = this.modelPointers[modelIndex];
            if (modelPointer.containedSegments == OccludingVoxelVertexSegement.None) continue;

            var shown = OccludingVoxelVertexSegement.None;
            shown |= IsSegmentHidden(pos, Direction.North, OccludingVoxelVertexSegement.South) ? 0 : OccludingVoxelVertexSegement.North;
            shown |= IsSegmentHidden(pos, Direction.East, OccludingVoxelVertexSegement.West) ? 0 : OccludingVoxelVertexSegement.East;
            shown |= IsSegmentHidden(pos, Direction.South, OccludingVoxelVertexSegement.North) ? 0 : OccludingVoxelVertexSegement.South;
            shown |= IsSegmentHidden(pos, Direction.West, OccludingVoxelVertexSegement.East) ? 0 : OccludingVoxelVertexSegement.West;
            shown |= IsSegmentHidden(pos, Direction.Up, OccludingVoxelVertexSegement.Bottom) ? 0 : OccludingVoxelVertexSegement.Top;
            shown |= IsSegmentHidden(pos, Direction.Down, OccludingVoxelVertexSegement.Top) ? 0 : OccludingVoxelVertexSegement.Bottom;

            if (shown > 0) shown |= OccludingVoxelVertexSegement.Center;
            if ((modelPointer.containedSegments & shown) == OccludingVoxelVertexSegement.None) continue;

            for (int i = 0; i < modelPointer.vertexCount; i++)
            {
              var vertex = this.blockModelAtlas.GetVertex(i + modelPointer.vertexOffset);
              this.chunkMesh.AppendVertex(new VoxelVertex()
              {
                position = vertex.position + new float3(x, y, z),
                normal = vertex.normal,
                tangent = vertex.tangent,
                uv = vertex.uv
              });
            }

            for (int i = 0; i < modelPointer.indexCount; i++)
            {
              var index = this.blockModelAtlas.GetIndex(i + modelPointer.indexOffset);
              var vertex = this.blockModelAtlas.GetVertex(index + modelPointer.vertexOffset);

              if ((shown & vertex.segement) == 0) continue;
              this.chunkMesh.AppendIndex(index);
            }
          }
        }
      }
    }


    /// <summary>
    /// Checks if the given block face is hidden by a neighboring block or not.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="face">The face of the block.</param>
    /// <param name="segment">The neighboring block's facing segment.</param>
    /// <returns>True if the face is hidden. False otherwise.</returns>
    private bool IsSegmentHidden(BlockPos pos, Direction face, OccludingVoxelVertexSegement segment)
    {
      var modelIndex = this.chunkData.GetLocalBlock(pos + face);
      if (modelIndex == 0) return false;
      modelIndex--;

      var modelPointer = this.modelPointers[modelIndex];
      return (modelPointer.occludingSegments & segment) > 0;
    }
  }
}
