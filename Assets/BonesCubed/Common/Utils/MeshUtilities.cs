using Unity.Mathematics;
using Bones3.Native;
using UnityEngine;

namespace Bones3.Util
{
  /// <summary>
  /// A collection of utility functions for working with mesh data.
  /// </summary>
  public static class MeshUtilities
  {
    /// <summary>
    /// Converts the Unity block mesh into an occluded block model and bakes it
    /// into the model atlas. Block models may be either segmentable or not.
    /// Segmentable models are usually simple shapes like a plain cube or stair
    /// case. Segmentable models will allow faces that touch the edge of the
    /// block bounds to be occluded by neighboring blocks and removed from the
    /// generated chunk mesh. More complex models may not handle segmentation as
    /// well, and should have segmentation disabled.
    ///
    /// This function does not handle determining occluding segments, which
    /// should be assigned manually.
    /// </summary>
    /// <param name="mesh">The Unity block model mesh.</param>
    /// <param name="atlas">The occluding block model atlas.</param>
    /// <param name="textureIndex">The texture index to use for this model.</param>
    /// <param name="allowSegmentation">Whether or not to attempt to segment the model into occludable parts.</param>
    /// <returns>The model pointer data.</returns>
    public static OccludingBlockModel BakeBlockModelIntoAtlas(Mesh mesh, NativeMesh<OccludingVoxelVertex, ushort> atlas, int textureIndex, bool allowSegmentation)
    {
      var model = new OccludingBlockModel();

      var vertices = mesh.vertices;
      var normals = mesh.normals;
      var tangents = mesh.tangents;
      var uvs = mesh.uv;
      var indices = mesh.triangles;

      model.containedSegments = OccludingVoxelVertexSegement.None;
      model.occludingSegments = OccludingVoxelVertexSegement.None;
      model.vertexOffset = atlas.VertexCount;
      model.indexOffset = atlas.IndexCount;
      model.vertexCount = vertices.Length;
      model.indexCount = indices.Length;

      for (int i = 0; i < vertices.Length; i++)
      {
        // TODO Validate segmentation
        var segment = OccludingVoxelVertexSegement.Center;
        if (allowSegmentation)
        {
          if (math.dot(normals[i], new float3(0, 0, 1)) > 0.9999 && Mathf.Abs(vertices[i].z - 1) < 0.0001) segment = OccludingVoxelVertexSegement.North;
          if (math.dot(normals[i], new float3(1, 0, 0)) > 0.9999 && Mathf.Abs(vertices[i].x - 1) < 0.0001) segment = OccludingVoxelVertexSegement.East;
          if (math.dot(normals[i], new float3(0, 0, -1)) > 0.9999 && Mathf.Abs(vertices[i].z - 0) < 0.0001) segment = OccludingVoxelVertexSegement.South;
          if (math.dot(normals[i], new float3(-1, 0, 0)) > 0.9999 && Mathf.Abs(vertices[i].x - 0) < 0.0001) segment = OccludingVoxelVertexSegement.West;
          if (math.dot(normals[i], new float3(0, 1, 0)) > 0.9999 && Mathf.Abs(vertices[i].y - 1) < 0.0001) segment = OccludingVoxelVertexSegement.Top;
          if (math.dot(normals[i], new float3(0, -1, 0)) > 0.9999 && Mathf.Abs(vertices[i].y - 0) < 0.0001) segment = OccludingVoxelVertexSegement.Bottom;
        }
        model.containedSegments |= segment;

        atlas.AppendVertex(new OccludingVoxelVertex()
        {
          position = vertices[i],
          normal = normals[i],
          tangent = tangents[i],
          uv = new float3(uvs[i], textureIndex),
          segement = segment
        });
      }

      for (int i = 0; i < indices.Length; i++)
        atlas.AppendIndex((ushort)indices[i]);

      return model;
    }
  }
}
