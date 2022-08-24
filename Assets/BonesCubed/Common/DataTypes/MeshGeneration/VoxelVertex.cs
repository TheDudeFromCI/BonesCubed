using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Collections;

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
    public NativeArray<VertexAttributeDescriptor> GetLayout()
    {
      var array = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp);
      array[0] = new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.Position, VertexAttributeFormat.Float32, 3);
      array[1] = new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);
      array[2] = new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4);
      array[3] = new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 3);
      return array;
    }


    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      return obj is VoxelVertex vertex && vertex == this;
    }


    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return HashCode.Combine(this.position, this.normal, this.uv, this.tangent);
    }


    /// <inheritdoc/>
    public override string ToString()
    {
      return $"VoxelVertex (Position: {this.position}, Normal: {this.normal}, UV: {this.uv}, Tangent: {this.tangent})";
    }


    /// <summary>
    /// Checks whether or not the two given vertices are equal.
    /// </summary>
    /// <param name="a">The first vertex.</param>
    /// <param name="b">The second vertex.</param>
    /// <returns>True if the vertices are equal, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(VoxelVertex a, VoxelVertex b)
    {
      return math.distancesq(a.position, b.position) < 0.001
          && math.distancesq(a.normal, b.normal) < 0.001
          && math.distancesq(a.uv, b.uv) < 0.001
          && math.distancesq(a.tangent, b.tangent) < 0.001;
    }


    /// <summary>
    /// Checks whether or not the two given vertices are inequal.
    /// </summary>
    /// <param name="a">The first vertex.</param>
    /// <param name="b">The second vertex.</param>
    /// <returns>True if the vertices are not equal, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(VoxelVertex a, VoxelVertex b)
    {
      return math.distancesq(a.position, b.position) >= 0.001
          || math.distancesq(a.normal, b.normal) >= 0.001
          || math.distancesq(a.uv, b.uv) >= 0.001
          || math.distancesq(a.tangent, b.tangent) >= 0.001;
    }
  }
}
