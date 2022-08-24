using Unity.Mathematics;

namespace Bones3
{
  public interface IInfiniteGrid3D<T> : IDataContainer
  {
    /// <summary>
    /// Gets the element at the given position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>The element.</returns>
    T GetElement(int3 position);


    /// <summary>
    /// Sets the element at the given position to the provided value.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="value">The new value.</param>
    void SetElement(int3 position, T value);
  }
}
