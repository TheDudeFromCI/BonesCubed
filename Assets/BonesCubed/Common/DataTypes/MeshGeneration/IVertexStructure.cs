using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Collections;

namespace Bones3
{
  public interface IVertexStructure
  {
    /// <summary>
    /// Gets the position of this vertex.
    /// </summary>
    float3 Position { get; }


    /// <summary>
    /// Gets the vertex data layout within the GPU for this vertex structure.
    /// </summary>
    /// <returns>The vertex layout.</returns>
    NativeArray<VertexAttributeDescriptor> GetLayout();
  }
}
