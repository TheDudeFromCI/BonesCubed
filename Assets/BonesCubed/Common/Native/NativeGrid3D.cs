using System;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Bones3.Native.Unsafe;

namespace Bones3.Native
{
  /// <summary>
  /// An unmanaged data storage container for storing three dimensional,
  /// blittable data.
  /// </summary>
  /// <typeparam name="T">The data type to store in this container.</typeparam>
  [BurstCompile]
  [NativeContainer]
  public unsafe struct NativeGrid3D<T> : IGrid3D<T>
    where T : unmanaged
  {
    /// <summary>
    /// A utility job for disposing UnsafeGrid3D data from within a job.
    /// </summary>
    [BurstCompile]
    private unsafe struct DisposeJob : IJob
    {
      [NativeDisableUnsafePtrRestriction]
      public void* data;

      /// <inheritdoc/>
      [BurstCompile]
      public void Execute()
      {
        ((UnsafeGrid3D<T>*)this.data)->Dispose();
      }
    }


    /// <summary>
    /// A wrapper for a NativeGrid3D that allows for concurrent writing. This
    /// is intended for use within IJobParallelFor use cases.
    /// </summary>
    [BurstCompile]
    [NativeContainer]
    [NativeContainerIsAtomicWriteOnly]
    public struct Concurrent
    {
      [NativeDisableUnsafePtrRestriction]
      private UnsafeGrid3D<T>* grid;


      /// <see cref="NativeGrid3D{T}.Region"/>
      public Region Region => this.grid->Region;


      /// <see cref="NativeGrid3D{T}.Length"/>
      public int Length => this.grid->Length;



#if ENABLE_UNITY_COLLECTIONS_CHECKS
      internal AtomicSafetyHandle m_Safety;
#endif


      /// <summary>
      /// Converts a NativeGrid3D{T} container into it's concurrent counterpart.
      /// </summary>
      /// <param name="container">The container to convert.</param>
      public static implicit operator NativeGrid3D<T>.Concurrent(NativeGrid3D<T> container)
      {
        NativeGrid3D<T>.Concurrent concurrent;
        concurrent.grid = container.grid;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(container.m_Safety);
        concurrent.m_Safety = container.m_Safety;
        AtomicSafetyHandle.UseSecondaryVersion(ref concurrent.m_Safety);
#endif

        return concurrent;
      }


      /// <see cref="NativeGrid3D{T}.SetElement(int, T)"/>
      [BurstCompile]
      [WriteAccessRequired]
      public void SetElement(int index, T value)
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

        this.grid->SetElement(index, value);
      }


      /// <see cref="NativeGrid3D{T}.SetElement(int3, T)"/>
      [BurstCompile]
      [WriteAccessRequired]
      public void SetElement(int3 index, T value)
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

        this.grid->SetElement(index, value);
      }
    }


    [NativeDisableUnsafePtrRestriction]
    private UnsafeGrid3D<T>* grid;

    private Allocator allocator;


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static int s_staticSafetyId;
    internal AtomicSafetyHandle m_Safety;

    [NativeSetClassTypeToNullOnSchedule]
    internal DisposeSentinel m_DisposeSentinel;
#endif


    /// <inheritdoc/>
    public Region Region => IsCreated ? this.grid->Region : default;


    /// <inheritdoc/>
    public int Length => IsCreated ? this.grid->Length : default;


    /// <inheritdoc/>
    public bool IsCreated => this.grid != null;


    /// <summary>
    /// Creates a new NativeGrid3D instance.
    /// </summary>
    /// <param name="region">The region that grid maps to.</param>
    /// <param name="allocator">The allocator to use for memory management.</param>
    /// <exception cref="ArgumentException">If the allocator is invalid.</exception>
    /// <exception cref="ArgumentException">If the grid size is not at least 1x1x1 units.</exception>
    public NativeGrid3D(Region region, Allocator allocator)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Create(out this.m_Safety, out this.m_DisposeSentinel, 0, allocator);
      if (s_staticSafetyId == 0) s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeGrid3D<T>>();
      AtomicSafetyHandle.SetStaticSafetyId(ref this.m_Safety, s_staticSafetyId);
#endif

      this.allocator = allocator;
      long byteCount = UnsafeUtility.SizeOf<UnsafeGrid3D<T>>();
      this.grid = (UnsafeGrid3D<T>*)UnsafeUtility.Malloc(byteCount, UnsafeUtility.AlignOf<UnsafeGrid3D<T>>(), allocator);
      UnsafeUtility.WriteArrayElement<UnsafeGrid3D<T>>(this.grid, 0, new UnsafeGrid3D<T>(region, allocator));
    }


    /// <inheritdoc/>
    [BurstCompile]
    public T GetElement(int index)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      return this.grid->GetElement(index);
    }


    /// <inheritdoc/>
    [BurstCompile]
    [WriteAccessRequired]
    public void SetElement(int index, T value)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      this.grid->SetElement(index, value);
    }


    /// <inheritdoc/>
    [BurstCompile]
    public T GetElement(int3 index)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      return this.grid->GetElement(index);
    }


    /// <inheritdoc/>
    [BurstCompile]
    [WriteAccessRequired]
    public void SetElement(int3 index, T value)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      this.grid->SetElement(index, value);
    }


    /// <inheritdoc/>
    [BurstCompile]
    public void Dispose()
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif

      this.grid->Dispose();

      UnsafeUtility.Free(this.grid, this.allocator);
      this.grid = null;
    }


    /// <summary>
    /// Causes this native container to be disposed when the provided job is
    /// completed.
    /// </summary>
    /// <param name="jobHandle">The job handle to wait for.</param>
    /// <returns>The modified job handle.</returns>
    public JobHandle Dispose(JobHandle jobHandle)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Clear(ref this.m_DisposeSentinel);
#endif

      jobHandle = new DisposeJob()
      {
        data = this.grid
      }.Schedule(jobHandle);
      UnsafeUtility.Free(this.grid, this.allocator);
      this.grid = null;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.Release(this.m_Safety);
#endif

      return jobHandle;
    }
  }
}
