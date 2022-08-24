using UnityEngine;

namespace Bones3
{
  /// <summary>
  /// Defines a collection of properties about a block type.
  /// </summary>
  public interface IBlockType
  {
    /// <summary>
    /// Gets the display name of this block type.
    /// </summary>
    string Name { get; }


    /// <summary>
    /// The material for this block type.
    /// </summary>
    Material Material { get; }


    /// <summary>
    /// The block model that this block type uses.
    /// </summary>
    IBlockModel BlockModel { get; }
  }
}
