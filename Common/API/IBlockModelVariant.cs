using UnityEngine;

namespace Bones3
{
  /// <summary>
  /// Represents a specific block model variant that can be applied to a block
  /// based off its current state and type.
  /// </summary>
  public interface IBlockModelVariant
  {
    /// <summary>
    /// Checks whether or not this block model variant is considered valid for
    /// the given block. This means that all conditions attached to this block
    /// model variant are met by the provided block.
    /// </summary>
    /// <param name="block">The block to check.</param>
    /// <returns>True if this model variant is valid for the block. False otherwise.</returns>
    bool IsValidFor(IBlock block);


    /// <summary>
    /// Gets the static mesh that is used for this block model variant.
    /// </summary>
    Mesh StaticMesh { get; }
  }
}
