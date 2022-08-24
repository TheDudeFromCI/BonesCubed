using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Bones3
{
  public static class NativeExtentions
  {
    /// <summary>
    /// Validates whether or not this data container has been created.
    /// </summary>
    /// <param name="container">The container to validate.</param>
    /// <exception cref="InvalidOperationException">If this data container is not created.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateAllocation<Container>(this Container container) where Container : IDataContainer
    {
      if (!container.IsCreated) throw new InvalidOperationException("Data container is not initialized");
    }


    /// <summary>
    /// Validates the given index and ensures that is lies within this grid.
    /// </summary>
    /// <param name="container">The container to validate.</param>
    /// <param name="index">The index to check.</param>
    /// <exception cref="IndexOutOfRangeException">If the index is outside of this grid.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateIndex<T, Container>(this Container grid, int index) where Container : IIndexedContainer<T>
    {
      if (index < 0 || index >= grid.Length) throw new IndexOutOfRangeException($"Index {index} is invalid for container of length {grid.Length}");
    }


    /// <summary>
    /// Validates the given position and ensures that it lies within this grid.
    /// </summary>
    /// <param name="container">The grid to validate.</param>
    /// <param name="pos">The position to check.</param>
    /// <exception cref="IndexOutOfRangeException">If the position is outside of this grid.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidatePosition<T, Container>(this Container grid, int3 pos) where Container : IGrid3D<T>
    {
      if (!grid.Region.Contains(pos)) throw new IndexOutOfRangeException($"Position {pos} is not inside region {grid.Region}");
    }
  }
}
