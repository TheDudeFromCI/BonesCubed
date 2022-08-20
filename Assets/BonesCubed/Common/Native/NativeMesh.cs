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
    where V : unmanaged
    where I : unmanaged
  {
    [NativeDisableUnsafePtrRestriction]
    private void* submeshList;

    private Allocator allocator;


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static int s_staticSafetyId;
    internal AtomicSafetyHandle m_Safety;

    [NativeSetClassTypeToNullOnSchedule]
    internal DisposeSentinel m_DisposeSentinel;
#endif


    /// <summary>
    /// A list of all the submeshes in this native mesh.
    /// </summary>
    public NativeContainerList<NativeSubmesh<V, I>> SubmeshList
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return UnsafeUtility.ReadArrayElement<NativeContainerList<NativeSubmesh<V, I>>>(this.submeshList, 0);
      }
    }


    /// <summary>
    /// Gets the submesh within this mesh at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The submesh.</returns>
    /// <exception cref="IndexOutOfRangeException">If the index is out of the bounds of the array.</exception>
    public NativeSubmesh<V, I> this[int index] => SubmeshList[index];


    /// <summary>
    /// Gets the total number of vertices in this mesh across all submeshes.
    /// </summary>
    public int TotalVertexCount
    {
      get
      {
        var count = 0;
        var submeshList = SubmeshList;
        for (int i = 0; i < submeshList.Count; i++) count += submeshList[i].VertexList.Length;
        return count;
      }
    }


    /// <summary>
    /// Gets the total number of indices in this mesh across all submeshes.
    /// </summary>
    public int TotalIndexCount
    {
      get
      {
        var count = 0;
        var submeshList = SubmeshList;
        for (int i = 0; i < submeshList.Count; i++) count += submeshList[i].IndexList.Length;
        return count;
      }
    }


    /// <summary>
    /// Gets the bounding box of this native mesh across all submeshes.
    /// </summary>
    public Bounds Bounds
    {
      get
      {
        var bounds = new Bounds();
        var submeshList = SubmeshList;
        for (int i = 0; i < submeshList.Count; i++)
        {
          if (bounds == default) bounds = submeshList[i].Bounds;
          else bounds.Encapsulate(submeshList[i].Bounds);
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
#endif

      this.allocator = allocator;

      long totalBytes = (long)UnsafeUtility.SizeOf<NativeContainerList<NativeSubmesh<V, I>>>();
      this.submeshList = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<NativeContainerList<NativeSubmesh<V, I>>>(), allocator);
      UnsafeUtility.WriteArrayElement<NativeContainerList<NativeSubmesh<V, I>>>(this.submeshList, 0, new NativeContainerList<NativeSubmesh<V, I>>(4, CreateNativeSubmesh, allocator));

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Create(out this.m_Safety, out this.m_DisposeSentinel, 0, allocator);
      if (s_staticSafetyId == 0) s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeMesh<V, I>>();
      AtomicSafetyHandle.SetStaticSafetyId(ref this.m_Safety, s_staticSafetyId);
#endif
    }


    /// <summary>
    /// Creates a new submesh instance using the given allocator.
    /// </summary>
    /// <param name="allocator">The allocator.</param>
    /// <returns>A new submesh instance.</returns>
    [BurstCompile]
    [MonoPInvokeCallback(typeof(BuildNativeContainer))]
    private unsafe static void CreateNativeSubmesh(void* buffer, int index, Allocator allocator)
    {
      var submesh = new NativeSubmesh<V, I>(allocator);
      UnsafeUtility.WriteArrayElement<NativeSubmesh<V, I>>(buffer, index, submesh);
    }


    /// <inheritdoc/>
    [WriteAccessRequired]
    public void Dispose()
    {
      SubmeshList.Dispose();
      UnsafeUtility.Free(this.submeshList, this.allocator);
      this.submeshList = null;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif
    }


    /// <summary>
    /// Gets all vertices in this native mesh across all submeshes and stores
    /// them in the given native array. Vertices are stored in the order of
    /// their submeshes. It is assumed that the provided array is at least
    /// as large as the value returned by TotalVertexCount.
    /// </summary>
    /// <param name="vertices">The array to write to.</param>
    [BurstCompile]
    public void GetAllVertices(NativeArray<V> vertices)
    {
      int index = 0;
      for (int i = 0; i < SubmeshList.Count; i++)
      {
        var vertexList = SubmeshList[i].VertexList;
        for (int j = 0; j < vertexList.Length; j++)
        {
          vertices[index++] = vertexList[j];
        }
      }
    }


    /// <summary>
    /// Gets all indices in this native mesh across all submeshes and stores
    /// them in the given native array. Indices are stored in the order of
    /// their submeshes. It is assumed that the provided array is at least
    /// as large as the value returned by TotalIndexCount.
    /// </summary>
    /// <param name="indices">The array to write to.</param>
    [BurstCompile]
    public void GetAllIndices(NativeArray<I> indices)
    {
      int index = 0;
      for (int i = 0; i < SubmeshList.Count; i++)
      {
        var indexList = SubmeshList[i].IndexList;
        for (int j = 0; j < indexList.Length; j++)
        {
          indices[index++] = indexList[j];
        }
      }
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

      var verts = new NativeArray<V>(TotalVertexCount, Allocator.Temp);
      var indis = new NativeArray<I>(TotalIndexCount, Allocator.Temp);
      GetAllVertices(verts);
      GetAllIndices(indis);

      mesh.Clear();
      mesh.subMeshCount = SubmeshList.Count;
      mesh.bounds = Bounds;
      mesh.SetVertexBufferParams(verts.Length, layout);
      mesh.SetVertexBufferData(verts, 0, 0, verts.Length);
      mesh.SetIndexBufferParams(indis.Length, typeof(I) == typeof(uint) ? IndexFormat.UInt32 : IndexFormat.UInt16);
      mesh.SetIndexBufferData(indis, 0, 0, indis.Length, updateFlags);
      verts.Dispose();
      indis.Dispose();

      int vertexOffset = 0;
      int indexOffset = 0;
      for (int s = 0; s < SubmeshList.Count; s++)
      {
        var submesh = SubmeshList[s];
        mesh.SetSubMesh(0, new SubMeshDescriptor()
        {
          indexCount = submesh.IndexList.Length,
          topology = MeshTopology.Triangles,
          baseVertex = vertexOffset,
          bounds = Bounds,
          firstVertex = indexOffset,
          vertexCount = submesh.VertexList.Length,
        }, updateFlags);

        vertexOffset += submesh.VertexList.Length;
        indexOffset += submesh.IndexList.Length;
      }
    }
  }
}
