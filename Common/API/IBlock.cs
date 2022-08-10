using System;

namespace Bones3
{
  /// <summary>
  /// Represents a collection of data stored within a single grid point in a
  /// world.
  /// </summary>
  public interface IBlock
  {
    /// <summary>
    /// Gets the position of this block within the world.
    /// </summary>
    BlockPos Position { get; }


    /// <summary>
    /// Gets the chunk this block is in.
    /// </summary>
    IChunk Chunk { get; }


    /// <summary>
    /// Gets the world this block is in.
    /// </summary>
    IWorld World { get => Chunk.World; }


    /// <summary>
    /// Gets or sets the current block type for this block. Changing the block
    /// type will reset all properties back to default for this block.
    /// </summary>
    IBlockType BlockType { get; set; }


    /// <summary>
    /// Gets or sets a property value for this block.
    /// </summary>
    /// <param name="property">The property to get/set.</param>
    /// <returns>The current property value.</returns>
    /// <exception cref="ArgumentException">The the property is not valid for the current block type.</exception>
    string this[string property] { get; set; }
  }
}
