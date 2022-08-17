using System;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace Bones3
{
  /// <summary>
  /// Represents a pointer data structure for loading block models from within
  /// the native reference mesh. This pointer is used to determine where the
  /// vertex data lies within the mesh.
  /// </summary>
  public struct OccludingBlockModel
  {
    /// <summary>
    /// The vertex offset position of this model.
    /// </summary>
    public int vertexOffset;


    /// <summary>
    /// The number of vertices in this model.
    /// </summary>
    public int vertexCount;


    /// <summary>
    /// The index offset position of this model.
    /// </summary>
    public int indexOffset;


    /// <summary>
    /// The number of indices in this model.
    /// </summary>
    public int indexCount;


    /// <summary>
    /// The segment types that are contained within this block model.
    /// </summary>
    public OccludingVoxelVertexSegement containedSegments;


    /// <summary>
    /// The sides of the block that are occluding by this model. This can be
    /// used to hide neighboring models.
    /// </summary>
    public OccludingVoxelVertexSegement occludingSegments;
  }


  /// <summary>
  /// An enum to indicate which segement of an occluding block model a vertex
  /// belongs to.
  /// </summary>
  [Flags]
  public enum OccludingVoxelVertexSegement
  {
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8,
    Top = 16,
    Bottom = 32,
    Center = 64
  }


  /// <summary>
  /// Data stored within a single vertex that is generated for a standard chunk
  /// mesh.
  /// </summary>
  public struct OccludingVoxelVertex : IVertexStructure
  {
    /// <summary>
    /// The position of this vertex.
    /// </summary>
    public float3 position;


    /// <summary>
    /// The normal of this vertex.
    /// </summary>
    public float3 normal;


    /// <summary>
    /// The tangent of this vertex.
    /// </summary>
    public float4 tangent;


    /// <summary>
    /// The uv of this vertex. (Note the the z coordinate corresponds to the
    /// index of the texture within the texture array,)
    /// </summary>
    public float3 uv;


    /// <summary>
    /// The segment of the model that this vertex belongs to.
    /// </summary>
    public OccludingVoxelVertexSegement segement;


    /// <inheritdoc/>
    public float3 Position => this.position;


    /// <inheritdoc/>
    public VertexAttributeDescriptor[] GetLayout()
    {
      throw new NotSupportedException("OccludingVoxelVertex can only be used for mesh generation references!");
    }
  }
}
