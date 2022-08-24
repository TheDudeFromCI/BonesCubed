using System;
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
  [NativeContainer]
  [NativeContainerSupportsDeallocateOnJobCompletion]
  public struct NativeInfiniteGrid3D<T> : IInfiniteGrid3D<T>
    where T : unmanaged
  {
    private UnsafeInfiniteGrid3D<T> grid;


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static int s_staticSafetyId;
    internal AtomicSafetyHandle m_Safety;

    [NativeSetClassTypeToNullOnSchedule]
    internal DisposeSentinel m_DisposeSentinel;
#endif


    /// <inheritdoc/>
    public bool IsCreated => this.grid.IsCreated;


    /// <summary>
    /// Creates a new NativeGrid3D instance.
    /// </summary>
    /// <param name="allocator">The allocator to use for memory management.</param>
    /// <exception cref="ArgumentException">If the allocator is invalid.</exception>
    public NativeInfiniteGrid3D(Allocator allocator)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Create(out this.m_Safety, out this.m_DisposeSentinel, 0, allocator);
      if (s_staticSafetyId == 0) s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeInfiniteGrid3D<T>>();
      AtomicSafetyHandle.SetStaticSafetyId(ref this.m_Safety, s_staticSafetyId);
#endif

      this.grid = new UnsafeInfiniteGrid3D<T>(allocator);
    }


    /// <inheritdoc/>
    public T GetElement(int3 index)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif

      return this.grid.GetElement(index);
    }


    /// <inheritdoc/>
    [WriteAccessRequired]
    public void SetElement(int3 index, T value)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      this.grid.SetElement(index, value);
    }


    /// <inheritdoc/>
    public void Dispose()
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif

      this.grid.Dispose();
    }
  }
}
