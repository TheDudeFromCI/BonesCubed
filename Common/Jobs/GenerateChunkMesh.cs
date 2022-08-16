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
    /// A 3x3x3 grid of chunks that contain block mesh data centered around the
    /// target chunk.
    /// </summary>
    [ReadOnly]
    public SurroundingChunkGrid<BlockMeshData> chunkData;


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
            if (!this.chunkData.GetLocalBlock(pos).IsSolid) continue;

            var center = new float3(x + 0.5f, y + 0.5f, z + 0.5f);
            var size = new float3(1f, 1f, 1f);
            var cube = new CubeMeshData(center, size);

            cube.northFace.textureIndex = this.chunkData.GetLocalBlock(pos + Direction.North).IsSolid ? -1 : 0;
            cube.eastFace.textureIndex = this.chunkData.GetLocalBlock(pos + Direction.East).IsSolid ? -1 : 0;
            cube.southFace.textureIndex = this.chunkData.GetLocalBlock(pos + Direction.South).IsSolid ? -1 : 0;
            cube.westFace.textureIndex = this.chunkData.GetLocalBlock(pos + Direction.West).IsSolid ? -1 : 0;
            cube.topFace.textureIndex = this.chunkData.GetLocalBlock(pos + Direction.Up).IsSolid ? -1 : 0;
            cube.bottomFace.textureIndex = this.chunkData.GetLocalBlock(pos + Direction.Down).IsSolid ? -1 : 0;

            MeshUtilities.AddCube(this.chunkMesh, cube);
          }
        }
      }
    }
  }
}
