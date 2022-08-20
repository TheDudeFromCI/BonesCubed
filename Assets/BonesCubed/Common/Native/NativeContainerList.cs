using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Bones3.Native
{
  /// <summary>
  /// A delegate for creating new native containers using a specified allocator.
  /// </summary>
  /// <param name="allocator">The allocator to use.</param>
  /// <returns>The new native container instance.</returns>
  public unsafe delegate void BuildNativeContainer(void* buffer, int index, Allocator allocator);


  /// <summary>
  /// A self-growing list of unmanaged data structures designed for containing,
  /// managing, and disposing other native containers. This native container
  /// attempts to handle most of the standard safety checks that go into into
  /// working with nested containers, however, to ensure better compatibiliy, it
  /// is recommended to never pass around a child instance alone, and to instead
  /// pass this entire list in and read the child directly from this list
  /// whenever access to the child is needed. This way all child elements can
  /// sure the same safety checks as this native container.
  /// </summary>
  /// <typeparam name="T">The type of data to store in this container.</typeparam>
  [BurstCompile]
  [NativeContainer]
  public unsafe struct NativeContainerList<T> : IDisposable where T : struct, IDisposable
  {
    [NativeDisableUnsafePtrRestriction]
    private void* buffer;

    [NativeDisableUnsafePtrRestriction]
    private void* capacity;

    [NativeDisableUnsafePtrRestriction]
    private void* count;

    private Allocator allocator;
    private FunctionPointer<BuildNativeContainer> containerSpawner;


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static int s_staticSafetyId;
    internal AtomicSafetyHandle m_Safety;

    [NativeSetClassTypeToNullOnSchedule]
    internal DisposeSentinel m_DisposeSentinel;
#endif


    /// <summary>
    /// Gets the current capacity for this container.
    /// </summary>
    public int Capacity
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return UnsafeUtility.ReadArrayElement<int>(this.capacity, 0);
      }

      private set
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif
        UnsafeUtility.WriteArrayElement<int>(this.capacity, 0, value);
      }
    }


    /// <summary>
    /// Gets the current number of elements in this container.
    /// </summary>
    public int Count
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
#endif
        return UnsafeUtility.ReadArrayElement<int>(this.count, 0);
      }

      private set
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif
        UnsafeUtility.WriteArrayElement<int>(this.count, 0, value);
      }
    }


    /// <summary>
    /// Gets the element within this container at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException">If the index is outside of the bounds of the array.</exception>
    public T this[int index]
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
        if (index < 0 || index >= Count) throw new IndexOutOfRangeException($"Index {index} is invalid for array of length {Count}!");
#endif
        return UnsafeUtility.ReadArrayElement<T>(this.buffer, index);
      }
    }


    /// <summary>
    /// Creates a new NativeSubmesh instance.
    ///
    /// A note on the container spawner function: This function must be a Burst
    /// compatible static, pure function. The function must also have the
    /// [BurstCompile] and [MonoPInvokeCallback(typeof(BuildNativeContainer))]
    /// attributes. The function should create a new native container instance
    /// and write it into the given buffer at the specified index.
    /// </summary>
    /// <param name="capacity">The initial capacity for this list.</param>
    /// <param name="containerSpawn">The function to use for creating new native container instances.</param>
    /// <param name="allocator">The allocator to use for memory management.</param>
    /// <exception cref="ArgumentException">If the allocator is invalid.</exception>
    /// <exception cref="ArgumentException">If the capacity is <= 0.</exception>
    public NativeContainerList(int capacity, BuildNativeContainer containerSpawner, Allocator allocator)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      if (allocator <= Allocator.None) throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
      if (capacity <= 0) throw new ArgumentException("Capacity must be greater than 0!", nameof(capacity));
#endif

      this.allocator = allocator;
      this.containerSpawner = BurstCompiler.CompileFunctionPointer<BuildNativeContainer>(containerSpawner);

      long totalBytes = (long)UnsafeUtility.SizeOf<int>();
      this.capacity = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<int>(), allocator);
      UnsafeUtility.WriteArrayElement<int>(this.capacity, 0, capacity);

      this.count = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<int>(), allocator);
      UnsafeUtility.WriteArrayElement<int>(this.count, 0, 0);

      totalBytes = (long)UnsafeUtility.SizeOf<T>() * capacity;
      this.buffer = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<T>(), allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Create(out this.m_Safety, out this.m_DisposeSentinel, 0, allocator);
      if (s_staticSafetyId == 0) s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeContainerList<T>>();
      AtomicSafetyHandle.SetStaticSafetyId(ref this.m_Safety, s_staticSafetyId);
#endif
    }


    /// <inheritdoc/>
    [WriteAccessRequired]
    public void Dispose()
    {
      for (int i = 0; i < Count; i++) this[i].Dispose();
      UnsafeUtility.Free(this.buffer, this.allocator);
      UnsafeUtility.Free(this.capacity, this.allocator);
      UnsafeUtility.Free(this.count, this.allocator);
      this.buffer = null;
      this.capacity = null;
      this.count = null;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif
    }


    /// <summary>
    /// Creates a new native container instance of T and adds it to this list.
    /// </summary>
    /// <returns>The new T instance.</returns>
    [BurstCompile]
    [WriteAccessRequired]
    public void SpawnNewInstance()
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

      if (Count == Capacity)
      {
        Capacity *= 2;
        long totalBytes = (long)UnsafeUtility.SizeOf<T>() * Capacity;
        var newBuffer = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<T>(), allocator);
        UnsafeUtility.MemCpy(newBuffer, this.buffer, Count);
        UnsafeUtility.Free(this.buffer, this.allocator);
        this.buffer = newBuffer;
      }

      this.containerSpawner.Invoke(this.buffer, Count, this.allocator);
      Count++;
    }
  }
}
