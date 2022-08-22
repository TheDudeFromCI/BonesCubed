using AOT;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Bones3.Native
{
  /// <summary>
  /// An unmanaged data storage container for storing mesh data that can be
  /// generated from within a Unity Job and transfered to a Mesh safely.
  /// </summary>
  [BurstCompile]
  [NativeContainer]
  public unsafe struct NativeMesh<V, I> : IDisposable
    where V : unmanaged, IVertexStructure
    where I : unmanaged
  {
    public struct SubmeshSize
    {
      public int vertexCount;
      public int indexCount;
      public Bounds bounds;
    }


    private NativeList<V> vertices;
    private NativeList<I> indices;
    private NativeList<SubmeshSize> submeshes;
    private Allocator allocator;


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static int s_staticSafetyId;
    internal AtomicSafetyHandle m_Safety;

    [NativeSetClassTypeToNullOnSchedule]
    internal DisposeSentinel m_DisposeSentinel;
#endif


    /// <summary>
    /// Gets the total number of vertices in this mesh across all submeshes.
    /// </summary>
    public int TotalVertexCount
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return this.vertices.Length;
      }
    }


    /// <summary>
    /// Gets the total number of indices in this mesh across all submeshes.
    /// </summary>
    public int TotalIndexCount
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return this.indices.Length;
      }
    }


    /// <summary>
    /// Gets the total number of submeshes in this mesh.
    /// </summary>
    public int SubmeshCount
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return this.submeshes.Length;
      }
    }


    /// <summary>
    /// Gets the bounding box of this native mesh across all submeshes.
    /// </summary>
    public Bounds Bounds
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        var bounds = new Bounds();
        for (int i = 0; i < this.submeshes.Length; i++)
        {
          if (bounds == default) bounds = this.submeshes[i].bounds;
          else bounds.Encapsulate(this.submeshes[i].bounds);
        }

        return bounds;
      }
    }


    /// <summary>
    /// Creates a new NativeMesh instance.
    /// </summary>
    /// <param name="allocator">The allocator to use for memory management.</param>
    /// <exception cref="ArgumentException">If the allocator is invalid.</exception>
    /// <exception cref="ArgumentException">If the genertic type V is not blittable.</exception>
    /// <exception cref="ArgumentException">If the genertic type V is not a IVertexStructure.</exception>
    /// <exception cref="ArgumentException">If the genertic type I is not a uint or ushort.</exception>
    public NativeMesh(Allocator allocator)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      if (allocator <= Allocator.None) throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
      if (!UnsafeUtility.IsBlittable<V>()) throw new ArgumentException($"{typeof(V)} used in NativeSubmesh<{typeof(V)},{typeof(I)}> must be blittable", nameof(V));
      if (!typeof(IVertexStructure).IsAssignableFrom(typeof(V))) throw new ArgumentException($"{typeof(V)} used in NativeSubmesh<{typeof(V)},{typeof(I)}> must extends the IVertexStructure interface", nameof(V));
      if (typeof(I) != typeof(uint) && typeof(I) != typeof(ushort)) throw new ArgumentException($"{typeof(I)} used in NativeSubmesh<{typeof(V)},{typeof(I)}> must be either a uint or a ushort", nameof(I));

      DisposeSentinel.Create(out this.m_Safety, out this.m_DisposeSentinel, 0, allocator);
      if (s_staticSafetyId == 0) s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeMesh<V, I>>();
      AtomicSafetyHandle.SetStaticSafetyId(ref this.m_Safety, s_staticSafetyId);
#endif

      this.allocator = allocator;
      this.vertices = new NativeList<V>(1024, allocator);
      this.indices = new NativeList<I>(2048, allocator);
      this.submeshes = new NativeList<SubmeshSize>(4, allocator);
    }


    /// <inheritdoc/>
    [WriteAccessRequired]
    public void Dispose()
    {
      this.vertices.Dispose();
      this.indices.Dispose();
      this.submeshes.Dispose();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif
    }


    /// <summary>
    /// Appends a new vertex to the given submesh.
    /// </summary>
    /// <param name="vertex">The vertex to add.</param>
    /// <param name="submesh">The submesh to add to.</param>
    [BurstCompile]
    [WriteAccessRequired]
    public void AddVertex(V vertex, int submesh)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      var vertexOffset = 0;
      for (int s = 0; s <= submesh; s++) vertexOffset += this.submeshes[s].vertexCount;
      Insert(this.vertices, vertex, vertexOffset);

      var sub = this.submeshes[submesh];
      sub.vertexCount++;
      sub.bounds.Encapsulate(vertex.Position);
      this.submeshes[submesh] = sub;
    }


    /// <summary>
    /// Appends a new index to the given submesh.
    /// </summary>
    /// <param name="index">The index to add.</param>
    /// <param name="submesh">The submesh to add to.</param>
    [BurstCompile]
    [WriteAccessRequired]
    public void AddIndex(I index, int submesh)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      var indexOffset = 0;
      for (int s = 0; s <= submesh; s++) indexOffset += this.submeshes[s].indexCount;
      Insert(this.indices, index, indexOffset);

      var sub = this.submeshes[submesh];
      sub.indexCount++;
      this.submeshes[submesh] = sub;
    }


    /// <summary>
    /// Gets the number of vertices in the given submesh.
    /// </summary>
    /// <param name="submesh">The submesh.</param>
    /// <returns>The vertex count.</returns>
    [BurstCompile]
    public int GetVertexCount(int submesh)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      return this.submeshes[submesh].vertexCount;
    }


    /// <summary>
    /// Gets the vertex at the given index within the indicated submesh.
    /// </summary>
    /// <param name="submesh">The submesh to read from.</param>
    /// <param name="index">The index of the vertex.</param>
    /// <returns>The vertex</returns>
    [BurstCompile]
    public V GetVertex(int submesh, int index)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      var vertexOffset = 0;
      for (int s = 0; s < submesh; s++) vertexOffset += this.submeshes[s].vertexCount;
      return this.vertices[vertexOffset + index];
    }


    /// <summary>
    /// Gets the index at the given index within the indicated submesh.
    /// </summary>
    /// <param name="submesh">The submesh to read from.</param>
    /// <param name="index">The index of the index.</param>
    /// <returns>The index</returns>
    [BurstCompile]
    public I GetIndex(int submesh, int index)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      var indexOffset = 0;
      for (int s = 0; s < submesh; s++) indexOffset += this.submeshes[s].indexCount;
      return this.indices[indexOffset + index];
    }


    /// <summary>
    /// Gets the number of indices in the given submesh.
    /// </summary>
    /// <param name="submesh">The submesh.</param>
    /// <returns>The index count.</returns>
    [BurstCompile]
    public int GetIndexCount(int submesh)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      return this.submeshes[submesh].indexCount;
    }


    /// <summary>
    /// Gets the bounds of the given submesh.
    /// </summary>
    /// <param name="submesh">The submesh.</param>
    /// <returns>The submesh bounds.</returns>
    [BurstCompile]
    public Bounds GetBounds(int submesh)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      return this.submeshes[submesh].bounds;
    }


    /// <summary>
    /// Appends a new submesh to this native mesh.
    /// </summary>
    [BurstCompile]
    [WriteAccessRequired]
    public void AppendSubmesh()
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      this.submeshes.Add(new SubmeshSize()
      {
        vertexCount = 0,
        indexCount = 0,
        bounds = default
      });
    }


    /// <summary>
    /// Converts the data within this native mesh and pushed it into a Unity
    /// mesh object.
    /// </summary>
    /// <param name="mesh">The Unity mesh object.</param>
    [BurstDiscard]
    public void ApplyToMesh(Mesh mesh)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      var layout = (new V() as IVertexStructure).GetLayout();
      var updateFlags = MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds;

      mesh.Clear();
      mesh.subMeshCount = SubmeshCount;
      mesh.bounds = Bounds;
      mesh.SetVertexBufferParams(TotalVertexCount, layout);
      mesh.SetVertexBufferData(this.vertices.AsArray(), 0, 0, TotalVertexCount);
      mesh.SetIndexBufferParams(TotalIndexCount, typeof(I) == typeof(uint) ? IndexFormat.UInt32 : IndexFormat.UInt16);
      mesh.SetIndexBufferData(this.indices.AsArray(), 0, 0, TotalIndexCount, updateFlags);

      int vertexOffset = 0;
      int indexOffset = 0;
      for (int s = 0; s < SubmeshCount; s++)
      {
        var submesh = this.submeshes[s];
        mesh.SetSubMesh(0, new SubMeshDescriptor()
        {
          indexCount = submesh.indexCount,
          topology = MeshTopology.Triangles,
          baseVertex = vertexOffset,
          bounds = Bounds,
          firstVertex = indexOffset,
          vertexCount = submesh.vertexCount,
        }, updateFlags);

        vertexOffset += submesh.vertexCount;
        indexOffset += submesh.indexCount;
      }
    }

    /// <summary>
    /// Insert an element into a list.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="item">The element.</param>
    /// <param name="index">The index.</param>
    private static unsafe void Insert<T>(NativeList<T> list, T item, int index) where T : unmanaged
    {
      if (index == list.Length)
      {
        list.Add(item);
        return;
      }

      if (index < 0 || index > list.Length) throw new IndexOutOfRangeException();
      list.Add(default);

      int elemSize = UnsafeUtility.SizeOf<T>();
      byte* basePtr = (byte*)list.GetUnsafePtr();

      var from = (index * elemSize) + basePtr;
      var to = (elemSize * (index + 1)) + basePtr;
      var size = elemSize * (list.Length - index - 1); // -1 because we added an extra fake element

      UnsafeUtility.MemMove(to, from, size);

      list[index] = item;
    }
  }
}
