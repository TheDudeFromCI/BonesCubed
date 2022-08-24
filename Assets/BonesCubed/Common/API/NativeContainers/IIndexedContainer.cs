namespace Bones3
{
  /// <summary>
  /// A type of data container that can be referenced with a finite index value.
  /// </summary>
  /// <typeparam name="T">The type of data in this container.</typeparam>
  public interface IIndexedContainer<T> : IDataContainer
  {
    /// <summary>
    /// Gets the total number of elements within this grid.
    /// </summary>
    int Length { get; }


    /// <summary>
    /// Gets the element at the given index within this grid.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The element.</returns>
    /// <exception cref="IndexOutOfRangeException">If the index is outside of this grid.</exception>
    /// <exception cref="InvalidOperationException">If this data container is not currently created.</exception>
    T GetElement(int index);


    /// <summary>
    /// Sets the element at the given index within this grid to the provided value.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="value">The new value.</param>
    /// <exception cref="IndexOutOfRangeException">If the index is outside of this grid.</exception>
    /// <exception cref="InvalidOperationException">If this data container is not currently created.</exception>
    void SetElement(int index, T value);
  }
}
