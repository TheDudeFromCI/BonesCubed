using System;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Bones3.Native
{
  /// <summary>
  /// An unmanged data storage container for storing the vertices and indices of
  /// a submesh.
  /// </summary>
  /// <typeparam name="V">The vertex data format.</typeparam>
  /// <typeparam name="I">The index data format. (Either uint or ushort)</typeparam>
  [BurstCompile]
  [NativeContainer]
  public unsafe struct NativeSubmesh<V, I> : IDisposable
    where V : unmanaged
    where I : unmanaged
  {
    [NativeDisableUnsafePtrRestriction]
    private void* vertexList;

    [NativeDisableUnsafePtrRestriction]
    private void* indexList;

    private Allocator allocator;


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static int s_staticSafetyId;
    internal AtomicSafetyHandle m_Safety;

    [NativeSetClassTypeToNullOnSchedule]
    internal DisposeSentinel m_DisposeSentinel;
#endif


    /// <summary>
    /// Gets the vertex list for this submesh.
    /// </summary>
    public NativeList<V> VertexList
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return UnsafeUtility.ReadArrayElement<NativeList<V>>(this.vertexList, 0);
      }
    }


    /// <summary>
    /// Gets the vertex list for this submesh.
    /// </summary>
    public NativeList<I> IndexList
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return UnsafeUtility.ReadArrayElement<NativeList<I>>(this.indexList, 0);
      }
    }


    /// <summary>
    /// Gets the bounding box of this submesh.
    /// </summary>
    public Bounds Bounds
    {
      get
      {
        var bounds = new Bounds();
        var vertexList = VertexList;
        for (int j = 0; j < vertexList.Length; j++)
        {
          var vertex = vertexList[j] as IVertexStructure;
          if (bounds == default) bounds = new Bounds(vertex.Position, Vector3.zero);
          else bounds.Encapsulate(vertex.Position);
        }

        return bounds;
      }
    }


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    [BurstDiscard]
    private static void SafetyValidation(Allocator allocator, AtomicSafetyHandle m_Safety)
    {
      if (allocator <= Allocator.None) throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
      if (!UnsafeUtility.IsBlittable<V>()) throw new ArgumentException($"{typeof(V)} used in NativeSubmesh<{typeof(V)},{typeof(I)}> must be blittable", nameof(V));
      if (!typeof(IVertexStructure).IsAssignableFrom(typeof(V))) throw new ArgumentException($"{typeof(V)} used in NativeSubmesh<{typeof(V)},{typeof(I)}> must extends the IVertexStructure interface", nameof(V));
      if (typeof(I) != typeof(uint) && typeof(I) != typeof(ushort)) throw new ArgumentException($"{typeof(I)} used in NativeSubmesh<{typeof(V)},{typeof(I)}> must be either a uint or a ushort", nameof(I));

      if (s_staticSafetyId == 0) s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeSubmesh<V, I>>();
      AtomicSafetyHandle.SetStaticSafetyId(ref m_Safety, s_staticSafetyId);
    }
#endif


    /// <summary>
    /// Creates a new NativeSubmesh instance.
    /// </summary>
    /// <param name="allocator">The allocator to use for memory management.</param>
    /// <exception cref="ArgumentException">If the allocator is invalid.</exception>
    /// <exception cref="ArgumentException">If the genertic type V is not blittable.</exception>
    /// <exception cref="ArgumentException">If the genertic type V is not a IVertexStructure.</exception>
    /// <exception cref="ArgumentException">If the genertic type I is not a uint or ushort.</exception>
    public NativeSubmesh(Allocator allocator)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 0, allocator);
      SafetyValidation(allocator, m_Safety);
#endif

      this.allocator = allocator;

      long totalBytes = (long)UnsafeUtility.SizeOf<NativeList<V>>();
      this.vertexList = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<NativeList<V>>(), allocator);
      UnsafeUtility.WriteArrayElement<NativeList<V>>(this.vertexList, 0, new NativeList<V>(1024, allocator));

      totalBytes = (long)UnsafeUtility.SizeOf<NativeList<I>>();
      this.indexList = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<NativeList<I>>(), allocator);
      UnsafeUtility.WriteArrayElement<NativeList<I>>(this.indexList, 0, new NativeList<I>(2048, allocator));
    }


    /// <inheritdoc/>
    [WriteAccessRequired]
    public void Dispose()
    {
      VertexList.Dispose();
      IndexList.Dispose();
      UnsafeUtility.Free(this.vertexList, this.allocator);
      UnsafeUtility.Free(this.indexList, this.allocator);
      this.vertexList = null;
      this.indexList = null;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif
    }
  }
}
