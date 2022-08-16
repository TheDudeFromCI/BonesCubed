using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine.Rendering;

namespace Bones3
{
  /// <summary>
  /// Data stored within a single vertex that is generated for a standard chunk
  /// mesh.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct VoxelVertex : IVertexStructure
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


    /// <inheritdoc/>
    public float3 Position => this.position;


    /// <inheritdoc/>
    public VertexAttributeDescriptor[] GetLayout()
    {
      return new[]
      {
        new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
        new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
        new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4),
        new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 3)
      };
    }
  }
}
