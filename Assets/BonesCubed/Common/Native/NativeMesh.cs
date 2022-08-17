using System;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;

namespace Bones3.Native
{
  internal struct MeshMetaData
  {
    internal int vertexCapacity;
    internal int indexCapacity;
    internal int vertexCount;
    internal int indexCount;
    internal Bounds bounds;
  }

  /// <summary>
  /// An unmanaged data storage container for storing mesh data that can be
  /// generated from within a Unity Job and transfered to a Mesh safely.
  /// </summary>
  [BurstCompile]
  [NativeContainer]
  public unsafe struct NativeMesh<V, I> : IDisposable
    where V : struct, IVertexStructure
    where I : struct
  {
    [NativeDisableUnsafePtrRestriction]
    private void* vertexBuffer;

    [NativeDisableUnsafePtrRestriction]
    private void* indexBuffer;

    [NativeDisableUnsafePtrRestriction]
    private void* metadata;

    private Allocator allocator;


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static int s_staticSafetyId;
    internal AtomicSafetyHandle m_Safety;

    [NativeSetClassTypeToNullOnSchedule]
    internal DisposeSentinel m_DisposeSentinel;
#endif


    /// <summary>
    /// Gets the mesh metadata.
    /// </summary>
    private MeshMetaData MetaData
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return UnsafeUtility.ReadArrayElement<MeshMetaData>(this.metadata, 0);
      }

      set
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif
        UnsafeUtility.WriteArrayElement<MeshMetaData>(this.metadata, 0, value);
      }
    }


    /// <summary>
    /// Gets the current vertex capacity of this native mesh.
    /// </summary>
    public int VertexCapacity
    {
      get => MetaData.vertexCapacity;
      private set
      {
        var data = MetaData;
        data.vertexCapacity = value;
        MetaData = data;
      }
    }


    /// <summary>
    /// Gets the current index capacity of this native mesh.
    /// </summary>
    public int IndexCapacity
    {
      get => MetaData.indexCapacity;
      private set
      {
        var data = MetaData;
        data.indexCapacity = value;
        MetaData = data;
      }
    }


    /// <summary>
    /// Gets the current vertex count within this native mesh.
    /// </summary>
    public int VertexCount
    {
      get => MetaData.vertexCount;
      private set
      {
        var data = MetaData;
        data.vertexCount = value;
        MetaData = data;
      }
    }


    /// <summary>
    /// Gets the current index count within this native mesh.
    /// </summary>
    public int IndexCount
    {
      get => MetaData.indexCount;
      private set
      {
        var data = MetaData;
        data.indexCount = value;
        MetaData = data;
      }
    }


    /// <summary>
    /// Gets the current bounds of this native mesh.
    /// </summary>
    public Bounds Bounds
    {
      get => MetaData.bounds;
      private set
      {
        var data = MetaData;
        data.bounds = value;
        MetaData = data;
      }
    }


    /// <summary>
    /// Creates a new NativeMesh instance.
    /// </summary>
    /// <param name="allocator">The allocator to use for memory management.</param>
    /// <exception cref="ArgumentException">If the allocator is invalid.</exception>
    public NativeMesh(Allocator allocator)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      if (allocator <= Allocator.None) throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
      if (!UnsafeUtility.IsBlittable<V>()) throw new ArgumentException($"{typeof(V)} used in NativeMesh<{typeof(V)},{typeof(I)}> must be blittable", nameof(V));
      if (typeof(I) != typeof(uint) && typeof(I) != typeof(ushort)) throw new ArgumentException($"{typeof(I)} used in NativeMesh<{typeof(V)},{typeof(I)}> must be either a uint or a ushort!", nameof(I));
#endif

      this.allocator = allocator;

      var vertexCapacity = 1024;
      var indexCapacity = 2048;

      long totalBytes = (long)UnsafeUtility.SizeOf<MeshMetaData>();
      this.metadata = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<MeshMetaData>(), allocator);

      totalBytes = (long)UnsafeUtility.SizeOf<V>() * vertexCapacity;
      this.vertexBuffer = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<V>(), allocator);

      totalBytes = (long)UnsafeUtility.SizeOf<I>() * indexCapacity;
      this.indexBuffer = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<I>(), allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Create(out this.m_Safety, out this.m_DisposeSentinel, 0, allocator);
      if (s_staticSafetyId == 0) s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeMesh<V, I>>();
      AtomicSafetyHandle.SetStaticSafetyId(ref this.m_Safety, s_staticSafetyId);
#endif

      MetaData = new MeshMetaData()
      {
        vertexCapacity = vertexCapacity,
        indexCapacity = indexCapacity,
        vertexCount = 0,
        indexCount = 0,
        bounds = default
      };
    }


    /// <inheritdoc/>
    public void Dispose()
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif

      UnsafeUtility.Free(this.vertexBuffer, this.allocator);
      UnsafeUtility.Free(this.indexBuffer, this.allocator);
      UnsafeUtility.Free(this.metadata, this.allocator);
      this.vertexBuffer = null;
      this.indexBuffer = null;
      this.metadata = null;
    }


    /// <summary>
    /// Appends a new vertex to this mesh.
    /// </summary>
    /// <param name="vertex">The vertex to append.</param>
    [BurstCompile]
    [WriteAccessRequired]
    public void AppendVertex(V vertex)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      if (VertexCount == VertexCapacity)
      {
        VertexCapacity *= 2;
        long totalBytes = (long)UnsafeUtility.SizeOf<V>() * VertexCapacity;
        var newBuffer = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<V>(), allocator);
        UnsafeUtility.MemCpy(newBuffer, this.vertexBuffer, VertexCount);
        UnsafeUtility.Free(this.vertexBuffer, this.allocator);
        this.vertexBuffer = newBuffer;
      }

      UnsafeUtility.WriteArrayElement<V>(this.vertexBuffer, VertexCount, vertex);
      VertexCount++;

      if (VertexCount == 1)
        Bounds = new Bounds(vertex.Position, Vector3.zero);
      else
      {
        var b = Bounds;
        b.Encapsulate(vertex.Position);
        Bounds = b;
      }
    }


    /// <summary>
    /// Appends a new index to this mesh.
    /// </summary>
    /// <param name="index">The index to append.</param>
    [BurstCompile]
    [WriteAccessRequired]
    public void AppendIndex(I index)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      if (IndexCount == IndexCapacity)
      {
        IndexCapacity *= 2;
        long totalBytes = (long)UnsafeUtility.SizeOf<I>() * IndexCapacity;
        var newBuffer = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<I>(), allocator);
        UnsafeUtility.MemCpy(newBuffer, this.indexBuffer, IndexCount);
        UnsafeUtility.Free(this.indexBuffer, this.allocator);
        this.indexBuffer = newBuffer;
      }

      UnsafeUtility.WriteArrayElement<I>(this.indexBuffer, IndexCount, index);
      IndexCount++;
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

      var layout = new V().GetLayout();
      var verts = new NativeArray<V>(VertexCount, Allocator.Temp);
      var indis = new NativeArray<I>(IndexCount, Allocator.Temp);
      var updateFlags = MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds;

      long totalBytes = (long)UnsafeUtility.SizeOf<V>() * VertexCapacity;
      UnsafeUtility.MemCpy(verts.GetUnsafePtr(), this.vertexBuffer, totalBytes);

      totalBytes = (long)UnsafeUtility.SizeOf<I>() * IndexCapacity;
      UnsafeUtility.MemCpy(indis.GetUnsafePtr(), this.indexBuffer, totalBytes);

      mesh.Clear();
      mesh.subMeshCount = 1;
      mesh.bounds = Bounds;
      mesh.SetVertexBufferParams(VertexCount, layout);
      mesh.SetVertexBufferData(verts, 0, 0, VertexCount);
      mesh.SetIndexBufferParams(IndexCount, typeof(I) == typeof(uint) ? IndexFormat.UInt32 : IndexFormat.UInt16);
      mesh.SetIndexBufferData(indis, 0, 0, IndexCount, updateFlags);
      mesh.SetSubMesh(0, new SubMeshDescriptor()
      {
        indexCount = IndexCount,
        topology = MeshTopology.Triangles,
        baseVertex = 0,
        bounds = Bounds,
        firstVertex = 0,
        vertexCount = VertexCount
      }, updateFlags);

      verts.Dispose();
      indis.Dispose();
    }


    /// <summary>
    /// Gets the vertex within this mesh at the specified index.
    /// </summary>
    /// <param name="index">The vertex index.</param>
    /// <returns>The vertex.</returns>
    public V GetVertex(int index)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      return UnsafeUtility.ReadArrayElement<V>(this.vertexBuffer, index);
    }


    /// <summary>
    /// Gets the index within this mesh at the specified index.
    /// </summary>
    /// <param name="index">The index's index.</param>
    /// <returns>The index.</returns>
    public I GetIndex(int index)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      return UnsafeUtility.ReadArrayElement<I>(this.indexBuffer, index);
    }
  }
}
