using System;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Bones3.Native.Unsafe
{
  public unsafe struct UnsafeGrid3D<T> : IGrid3D<T>
    where T : unmanaged
  {
    [NativeDisableUnsafePtrRestriction]
    private void* buffer;
    private Region region;
    private Allocator allocator;


    /// <inheritdoc/>
    public Region Region => this.region;


    /// <inheritdoc/>
    public int Length => this.region.Length;


    /// <inheritdoc/>
    public bool IsCreated => this.buffer != null;


    /// <summary>
    /// Creates a new UnsafeGrid3D instance.
    /// </summary>
    /// <param name="region">The region that grid maps to.</param>
    /// <param name="allocator">The allocator to use for memory management.</param>
    public UnsafeGrid3D(Region region, Allocator allocator)
    {
      if (allocator <= Allocator.None) throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
      if (region.size.x <= 0 || region.size.y <= 0 || region.size.z <= 0) throw new ArgumentException("Grid size must be at least 1x1x1 units", nameof(region));

      this.region = region;
      this.allocator = allocator;

      long byteCount = (long)UnsafeUtility.SizeOf<T>() * region.Length;
      this.buffer = UnsafeUtility.Malloc(byteCount, UnsafeUtility.AlignOf<T>(), allocator);
      UnsafeUtility.MemClear(this.buffer, byteCount);
    }


    /// <inheritdoc/>
    public T GetElement(int index)
    {
      this.ValidateAllocation();
      this.ValidateIndex<T, UnsafeGrid3D<T>>(index);
      return UnsafeUtility.ReadArrayElement<T>(this.buffer, index);
    }


    /// <inheritdoc/>
    public void SetElement(int index, T value)
    {
      this.ValidateAllocation();
      this.ValidateIndex<T, UnsafeGrid3D<T>>(index);
      UnsafeUtility.WriteArrayElement<T>(this.buffer, index, value);
    }


    /// <inheritdoc/>
    public T GetElement(int3 pos)
    {
      this.ValidateAllocation();
      this.ValidatePosition<T, UnsafeGrid3D<T>>(pos);

      int index = this.region.IndexFromPosition(pos);
      return UnsafeUtility.ReadArrayElement<T>(this.buffer, index);
    }


    /// <inheritdoc/>
    public void SetElement(int3 pos, T value)
    {
      this.ValidateAllocation();
      this.ValidatePosition<T, UnsafeGrid3D<T>>(pos);

      int index = this.region.IndexFromPosition(pos);
      UnsafeUtility.WriteArrayElement<T>(this.buffer, index, value);
    }


    /// <inheritdoc/>
    public void Dispose()
    {
      if (!IsCreated) return;
      UnsafeUtility.Free(this.buffer, this.allocator);
      this.buffer = null;
    }
  }
}
