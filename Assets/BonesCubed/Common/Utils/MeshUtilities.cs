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
    /// <param name="blockModel">The Unity block model.</param>
    /// <param name="atlas">The occluding block model atlas.</param>
    /// <param name="textureIndex">The texture index to use for this model.</param>
    /// <param name="allowSegmentation">Whether or not to attempt to segment the model into occludable parts.</param>
    /// <returns>The model pointer data.</returns>
    public static OccludingBlockModel BakeBlockModelIntoAtlas(IBlockModel blockModel, NativeMesh<OccludingVoxelVertex, ushort> atlas, int textureIndex, bool allowSegmentation)
    {
      var model = new OccludingBlockModel();

      var mesh = blockModel.StaticMesh;
      var vertices = mesh.vertices;
      var normals = mesh.normals;
      var tangents = mesh.tangents;
      var uvs = mesh.uv;
      var indices = mesh.triangles;

      model.containedSegments = OccludingVoxelVertexSegement.None;
      model.occludingSegments = blockModel.OccludingDirections;
      model.vertexOffset = atlas.TotalVertexCount;
      model.indexOffset = atlas.TotalIndexCount;
      model.vertexCount = vertices.Length;
      model.indexCount = indices.Length;

      if (atlas.SubmeshList.Count == 0) atlas.SubmeshList.SpawnNewInstance();
      var submesh = atlas[0];

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

        submesh.VertexList.Add(new OccludingVoxelVertex()
        {
          position = vertices[i],
          normal = normals[i],
          tangent = tangents[i],
          uv = new float3(uvs[i], textureIndex),
          segement = segment
        });
      }

      for (int i = 0; i < indices.Length; i++)
        submesh.IndexList.Add((ushort)indices[i]);

      return model;
    }
  }
}
