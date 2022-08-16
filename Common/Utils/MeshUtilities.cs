using Unity.Burst;
using Unity.Mathematics;
using Bones3.Native;

namespace Bones3
{
  public static class MeshUtilities
  {
    /// <summary>
    /// Generates a cube object based on the given mesh data and adds it to the
    /// given mesh.
    /// </summary>
    /// <param name="mesh">The mesh</param>
    /// <param name="cube">The cube data to apply.</param>
    [BurstCompile]
    public static void AddCube(NativeMesh<VoxelVertex, ushort> mesh, CubeMeshData cube)
    {
      var center = cube.center;
      var half = cube.size / 2;

      var v0 = new float3(center.x - half.x, center.y - half.y, center.z - half.z);
      var v1 = new float3(center.x - half.x, center.y - half.y, center.z + half.z);
      var v2 = new float3(center.x - half.x, center.y + half.y, center.z - half.z);
      var v3 = new float3(center.x - half.x, center.y + half.y, center.z + half.z);
      var v4 = new float3(center.x + half.x, center.y - half.y, center.z - half.z);
      var v5 = new float3(center.x + half.x, center.y - half.y, center.z + half.z);
      var v6 = new float3(center.x + half.x, center.y + half.y, center.z - half.z);
      var v7 = new float3(center.x + half.x, center.y + half.y, center.z + half.z);

      AddQuad(mesh, v7, v3, v1, v5, new float3(0, 0, -1), cube.northFace);
      AddQuad(mesh, v6, v7, v5, v4, new float3(1, 0, 0), cube.eastFace);
      AddQuad(mesh, v2, v6, v4, v0, new float3(0, 0, 1), cube.southFace);
      AddQuad(mesh, v3, v2, v0, v1, new float3(-1, 0, 0), cube.westFace);
      AddQuad(mesh, v3, v7, v6, v2, new float3(0, 1, 0), cube.topFace);
      AddQuad(mesh, v0, v4, v5, v1, new float3(0, -1, 0), cube.bottomFace);
    }


    /// <summary>
    /// Generates a quad and appends it to the output mesh.
    /// </summary>
    /// <param name="mesh">The mesh</param>
    /// <param name="v0">The first vertex position.</param>
    /// <param name="v1">The second vertex position.</param>
    /// <param name="v2">The third vertex position.</param>
    /// <param name="v3">The fourth vertex position.</param>
    /// <param name="normal">The normal of the quad.</param>
    /// <param name="quad">The quad texture data.</param>
    [BurstCompile]
    private static void AddQuad(NativeMesh<VoxelVertex, ushort> mesh, float3 v0, float3 v1, float3 v2, float3 v3, float3 normal, QuadMeshData quad)
    {
      if (quad.textureIndex < 0) return;

      int vertexCount = mesh.VertexCount;
      mesh.AppendIndex((ushort)(vertexCount + 0));
      mesh.AppendIndex((ushort)(vertexCount + 1));
      mesh.AppendIndex((ushort)(vertexCount + 2));
      mesh.AppendIndex((ushort)(vertexCount + 0));
      mesh.AppendIndex((ushort)(vertexCount + 2));
      mesh.AppendIndex((ushort)(vertexCount + 3));


      // TODO Improve this tangent generation code?
      float3 edge1 = v0 - v1;
      float3 edge2 = v2 - v0;
      float deltaV0 = quad.uv0.y - quad.uv1.y;
      float deltaV1 = quad.uv2.y - quad.uv0.y;
      float deltaU0 = quad.uv0.x - quad.uv1.x;
      float deltaU1 = quad.uv2.x - quad.uv0.x;
      float4 tangent = new float4(math.normalize(deltaV1 * edge1 - deltaV0 * edge2), 1);
      float3 binormal = math.normalize(deltaU1 * edge1 - deltaU0 * edge2);
      if (math.dot(math.cross(tangent.xyz, binormal), normal) < 0) tangent = -tangent;


      mesh.AppendVertex(new VoxelVertex()
      {
        position = v0,
        normal = normal,
        tangent = tangent,
        uv = new float3(quad.uv0, quad.textureIndex)
      });
      mesh.AppendVertex(new VoxelVertex()
      {
        position = v1,
        normal = normal,
        tangent = tangent,
        uv = new float3(quad.uv1, quad.textureIndex)
      });
      mesh.AppendVertex(new VoxelVertex()
      {
        position = v2,
        normal = normal,
        tangent = tangent,
        uv = new float3(quad.uv2, quad.textureIndex)
      });
      mesh.AppendVertex(new VoxelVertex()
      {
        position = v3,
        normal = normal,
        tangent = tangent,
        uv = new float3(quad.uv3, quad.textureIndex)
      });
    }
  }
}
