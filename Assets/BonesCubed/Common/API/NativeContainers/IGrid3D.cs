using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Bones3
{
  /// <summary>
  /// A finite 3D grid of elements.
  /// </summary>
  /// <typeparam name="T">The type of element in this grid.</typeparam>
  public interface IGrid3D<T> : IIndexedContainer<T>
  {
    /// <summary>
    /// Gets the region that this grid maps to.
    /// </summary>
    Region Region { get; }


    /// <summary>
    /// Gets the element at the given position within this grid.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <returns>The element.</returns>
    /// <exception cref="IndexOutOfRangeException">If the position is outside of this grid.</exception>
    /// <exception cref="InvalidOperationException">If this data container is not currently created.</exception>
    T GetElement(int3 pos);


    /// <summary>
    /// Sets the element at the given position within this grid to the provided value.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <param name="value">The new value.</param>
    /// <returns>The element.</returns>
    /// <exception cref="IndexOutOfRangeException">If the position is outside of this grid.</exception>
    /// <exception cref="InvalidOperationException">If this data container is not currently created.</exception>
    void SetElement(int3 pos, T value);
  }
}
