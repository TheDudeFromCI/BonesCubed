using System;
using LibSugar;

namespace Bones3
{
  /// <summary>
  /// A collection of possible block models that may be applied to a given block
  /// for a given block type. The variant that is used is based of a set of
  /// conditions that must be met.
  /// </summary>
  public interface IBlockModel
  {
    /// <summary>
    /// Gets the best model variant for the given block.
    /// </summary>
    /// <param name="block">The block to choose the variant of.</param>
    /// <returns>The first available block model variant, or none if there are not available variants.</returns>
    /// <exception cref="ArgumentException">If a varient relies on block properties that are not valid for the given block.</exception>
    Option<IBlockModelVariant> GetVariant(IBlock block);
  }
}
