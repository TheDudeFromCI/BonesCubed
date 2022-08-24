using System;

namespace Bones3
{
  /// <summary>
  /// A type of container that stores unsafe data directly.
  /// </summary>
  public interface IDataContainer : IDisposable
  {
    /// <summary>
    /// Gets whether or not this data container is allocated or not.
    /// </summary>
    bool IsCreated { get; }
  }
}
