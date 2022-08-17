using Unity.Mathematics;

namespace Bones3
{
  /// <summary>
  /// Contains texture information about a quad.
  /// </summary>
  public struct QuadMeshData
  {
    /// <summary>
    /// The texture index to apply to this quad. If index is negative, then this
    /// quad is not included in the mesh.
    /// </summary>
    public int textureIndex;


    /// <summary>
    /// The UV coordinates for the top left corner.
    /// </summary>
    public float2 uv0;


    /// <summary>
    /// The UV coordinates for the top right corner.
    /// </summary>
    public float2 uv1;


    /// <summary>
    /// The UV coordinates for the bottom right corner.
    /// </summary>
    public float2 uv2;


    /// <summary>
    /// The UV coordinates for the bottom left corner.
    /// </summary>
    public float2 uv3;


    /// <summary>
    /// Creates a new QuadMesh instance with default uv coordinates.
    /// </summary>
    /// <param name="textureIndex">The texture index of this quad.</param>
    public QuadMeshData(int textureIndex)
    {
      this.textureIndex = textureIndex;
      this.uv0 = new float2(0, 0);
      this.uv1 = new float2(0, 1);
      this.uv2 = new float2(1, 1);
      this.uv3 = new float2(1, 0);
    }
  }


  /// <summary>
  /// Contains positioning and texture information about a cube that can be
  /// placed within a mesh.
  /// </summary>
  public struct CubeMeshData
  {
    /// <summary>
    /// The center of the cube in local space.
    /// </summary>
    public float3 center;


    /// <summary>
    /// The size of the cube in local space.
    /// </summary>
    public float3 size;


    /// <summary>
    /// Texture data for the north-facing (in local space) quad.
    /// </summary>
    public QuadMeshData northFace;


    /// <summary>
    /// Texture data for the east-facing (in local space) quad.
    /// </summary>
    public QuadMeshData eastFace;


    /// <summary>
    /// Texture data for the south-facing (in local space) quad.
    /// </summary>
    public QuadMeshData southFace;


    /// <summary>
    /// Texture data for the west-facing (in local space) quad.
    /// </summary>
    public QuadMeshData westFace;


    /// <summary>
    /// Texture data for the top-facing (in local space) quad.
    /// </summary>
    public QuadMeshData topFace;


    /// <summary>
    /// Texture data for the bottom-facing (in local space) quad.
    /// </summary>
    public QuadMeshData bottomFace;


    /// <summary>
    /// Creates a new cube mesh data instance for the given size with default uv
    /// coordinates, and the texture index for each face set to 0.
    /// </summary>
    /// <param name="center">The center of the cube.</param>
    /// <param name="size">The size of the cube.</param>
    public CubeMeshData(float3 center, float3 size)
    {
      this.center = center;
      this.size = size;
      this.northFace = new QuadMeshData(0);
      this.eastFace = new QuadMeshData(0);
      this.southFace = new QuadMeshData(0);
      this.westFace = new QuadMeshData(0);
      this.topFace = new QuadMeshData(0);
      this.bottomFace = new QuadMeshData(0);
    }
  }
}
