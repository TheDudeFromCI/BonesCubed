using System;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Bones3.Native.Unsafe
{
  public unsafe struct UnsafeInfiniteGrid3D<T> : IInfiniteGrid3D<T>
    where T : unmanaged
  {
    private UnsafeParallelHashMap<int3, UnsafeGrid3D<T>> chunks;
    private Allocator allocator;


    public bool IsCreated => this.chunks.IsCreated;


    /// <summary>
    /// Creates a new UnsafeInfiniteGrid3D instance.
    /// </summary>
    /// <param name="allocator">The allocator to use for memory management.</param>
    public UnsafeInfiniteGrid3D(Allocator allocator)
    {
      if (allocator <= Allocator.None) throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));

      this.chunks = new UnsafeParallelHashMap<int3, UnsafeGrid3D<T>>(64, allocator);
      this.allocator = allocator;
    }


    /// <inheritdoc/>
    public T GetElement(int3 pos)
    {
      this.ValidateAllocation();

      var chunkIndex = pos >> 4;
      if (!this.chunks.ContainsKey(chunkIndex)) return default;

      return this.chunks[chunkIndex].GetElement(pos);
    }


    /// <inheritdoc/>
    public void SetElement(int3 pos, T value)
    {
      this.ValidateAllocation();

      var chunkIndex = pos >> 4;
      if (!this.chunks.ContainsKey(chunkIndex)) this.chunks.Add(chunkIndex, new UnsafeGrid3D<T>(new Region(chunkIndex << 4, 16), this.allocator));

      this.chunks[chunkIndex].SetElement(pos, value);
    }


    /// <inheritdoc/>
    public void Dispose()
    {
      if (!IsCreated) return;

      var chunkList = this.chunks.GetValueArray(Allocator.Temp);
      for (int i = 0; i < chunkList.Length; i++) chunkList[i].Dispose();
      chunkList.Dispose();

      this.chunks.Dispose();
    }
  }
}
