using System.Collections.ObjectModel;

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
    /// Gets whether or not this block type has a renderable, static mesh. If
    /// this block type is a tile entity or is not visibile, this value should
    /// return false. True otherwise.
    /// </summary>
    bool HasStaticMesh { get; }


    /// <summary>
    /// Gets a read-only dictionary of default properties defined by this block
    /// type. Any properties that are not listed in this dictionary cannot be
    /// provided to this block type.
    /// </summary>
    ReadOnlyDictionary<string, string> DefaultProperties { get; }
  }
}
