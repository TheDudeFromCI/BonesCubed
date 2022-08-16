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
    [ReadOnly]
    public NativeGrid3D<BlockMeshData> blockMeshData;


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
            if (!this.blockMeshData[pos].IsSolid) continue;

            var center = new float3(x + 0.5f, y + 0.5f, z + 0.5f);
            var size = new float3(1f, 1f, 1f);
            var cube = new CubeMeshData(center, size);
            MeshUtilities.AddCube(this.chunkMesh, cube);
          }
        }
      }
    }
  }
}
