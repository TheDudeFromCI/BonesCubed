using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Bones3.Native
{
  /// <summary>
  /// An unmanaged data storage container for storing three dimensional,
  /// blittable data.
  /// </summary>
  /// <typeparam name="T">The data type to store in this container.</typeparam>
  [NativeContainer]
  public unsafe struct NativeGrid3D<T> : IDisposable where T : struct
  {
    private void* buffer;
    private BlockPos size;
    private Allocator allocator;


    /// <summary>
    /// Gets the size of this grid.
    /// </summary>
    public BlockPos Size { get => this.size; }


    /// <summary>
    /// Gets or sets and element in this 3D array at the specified position.
    /// </summary>
    /// <param name="pos">The position within the grid.</param>
    /// <returns>The element at the given position.</returns>
    /// <exception cref="IndexOutOfRangeException">If the position is outside of grid bounds.</exception>
    public unsafe T this[BlockPos pos]
    {
      get
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckReadAndThrow(this.m_Safety);
        if (pos < new BlockPos(0, 0, 0) || pos >= this.size) throw new IndexOutOfRangeException($"{pos} is outside of grid bounds! Size: {this.size}");
#endif

        int index = pos.z * this.size.x * this.size.y + pos.y * this.size.x + pos.x;
        return UnsafeUtility.ReadArrayElement<T>(this.buffer, index);
      }

      set
      {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
        if (pos < new BlockPos(0, 0, 0) || pos >= this.size) throw new IndexOutOfRangeException($"{pos} is outside of grid bounds! Size: {this.size}");
#endif

        int index = pos.z * this.size.x * this.size.y + pos.y * this.size.x + pos.x;
        UnsafeUtility.WriteArrayElement(this.buffer, index, value);
      }
    }


#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static int s_staticSafetyId;
    internal AtomicSafetyHandle m_Safety;
    internal DisposeSentinel m_DisposeSentinel;
#endif


    /// <summary>
    /// Creates a new NativeGrid3D instance.
    /// </summary>
    /// <param name="size">The size of the 3D grid.</param>
    /// <param name="allocator">The allocator to use for memory management.</param>
    /// <exception cref="ArgumentException">If the allocator is invalid.</exception>
    /// <exception cref="ArgumentException">If the grid size is not at least 1x1x1 units.</exception>
    /// <exception cref="ArgumentException">If the genertic type T is not blittable.</exception>
    public NativeGrid3D(BlockPos size, Allocator allocator)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      if (allocator <= Allocator.None) throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
      if (size.x <= 0 || size.y <= 0 || size.z <= 0) throw new ArgumentException("Grid size must be at least 1x1x1 units", nameof(size));
      if (!UnsafeUtility.IsBlittable<T>()) throw new ArgumentException($"{typeof(T)} used in NativeCustomArray<{typeof(T)}> must be blittable", nameof(T));
#endif

      long totalBytes = (long)UnsafeUtility.SizeOf<T>() * size.x * size.y * size.z;
      this.buffer = UnsafeUtility.Malloc(totalBytes, UnsafeUtility.AlignOf<T>(), allocator);
      UnsafeUtility.MemClear(this.buffer, totalBytes);

      this.size = size;
      this.allocator = allocator;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Create(out this.m_Safety, out this.m_DisposeSentinel, 0, allocator);
      if (s_staticSafetyId == 0) s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeGrid3D<T>>();
      AtomicSafetyHandle.SetStaticSafetyId(ref this.m_Safety, s_staticSafetyId);
#endif
    }


    /// <inheritdoc/>
    public void Dispose()
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
      DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif

      UnsafeUtility.Free(this.buffer, this.allocator);
    }
  }
}
