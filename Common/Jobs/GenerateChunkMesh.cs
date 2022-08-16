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
            GenerateCube(pos);
          }
        }
      }
    }

    [BurstCompile]
    private void GenerateCube(BlockPos pos)
    {
      float3 cubePosition = pos.AsVector3;
      GenerateQuad(cubePosition, new float3(0, 1, 0), 0);
      GenerateQuad(cubePosition, new float3(0, -1, 0), 0);
      GenerateQuad(cubePosition, new float3(1, 0, 0), 0);
      GenerateQuad(cubePosition, new float3(-1, 0, 0), 0);
      GenerateQuad(cubePosition, new float3(0, 0, 1), 0);
      GenerateQuad(cubePosition, new float3(0, 0, -1), 0);
    }


    [BurstCompile]
    private void GenerateQuad(float3 cubePosition, float3 normal, int textureIndex)
    {
      int vertexCount = this.chunkMesh.VertexCount;
      this.chunkMesh.AppendIndex((ushort)(vertexCount + 0));
      this.chunkMesh.AppendIndex((ushort)(vertexCount + 1));
      this.chunkMesh.AppendIndex((ushort)(vertexCount + 2));
      this.chunkMesh.AppendIndex((ushort)(vertexCount + 0));
      this.chunkMesh.AppendIndex((ushort)(vertexCount + 2));
      this.chunkMesh.AppendIndex((ushort)(vertexCount + 3));

      float3 center = cubePosition + new float3(0.5f, 0.5f, 0.5f);
      quaternion axis = quaternion.AxisAngle(normal, math.PI / 2);

      float3 v0 = cubePosition + math.max(normal, float3.zero);
      float3 v1 = math.mul(axis, v0 - center) + center;
      float3 v2 = math.mul(axis, v1 - center) + center;
      float3 v3 = math.mul(axis, v2 - center) + center;

      float3 uv0 = new float3(1, 1, textureIndex);
      float3 uv1 = new float3(1, 0, textureIndex);
      float3 uv2 = new float3(0, 0, textureIndex);
      float3 uv3 = new float3(0, 1, textureIndex);

      float3 edge1 = v0 - v1;
      float3 edge2 = v2 - v0;
      float deltaV0 = uv0.y - uv1.y;
      float deltaV1 = uv2.y - uv0.y;
      float4 tangent = new float4(math.normalize(deltaV1 * edge1 - deltaV0 * edge2), 1);

      float deltaU0 = uv0.x - uv1.x;
      float deltaU1 = uv2.x - uv0.x;
      float3 binormal = math.normalize(deltaU1 * edge1 - deltaU0 * edge2);

      if (math.dot(math.cross(tangent.xyz, binormal), normal) < 0)
        tangent = -tangent;

      this.chunkMesh.AppendVertex(new VoxelVertex()
      {
        position = v0,
        normal = normal,
        tangent = tangent,
        uv = uv0
      });
      this.chunkMesh.AppendVertex(new VoxelVertex()
      {
        position = v1,
        normal = normal,
        tangent = tangent,
        uv = uv1
      });
      this.chunkMesh.AppendVertex(new VoxelVertex()
      {
        position = v2,
        normal = normal,
        tangent = tangent,
        uv = uv2
      });
      this.chunkMesh.AppendVertex(new VoxelVertex()
      {
        position = v3,
        normal = normal,
        tangent = tangent,
        uv = uv3
      });
    }
  }
}
