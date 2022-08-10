using System;
using System.Collections.Generic;

namespace Bones3.Storage
{
  internal class Block : IBlock
  {
    private Dictionary<string, string> properties = new Dictionary<string, string>();
    private IBlockType blockType;


    /// <inheritdoc/>
    public IChunk Chunk { get; }


    /// <inheritdoc/>
    public BlockPos Position { get; }


    /// <inheritdoc/>
    public IBlockType BlockType
    {
      get => this.blockType;
      set
      {
        if (value == null) throw new ArgumentNullException(nameof(value));
        this.blockType = value;
        properties.Clear();
      }
    }


    /// <inheritdoc/>
    public string this[string property]
    {
      get
      {
        if (this.properties.ContainsKey(property)) return this.properties[property];
        if (this.blockType.DefaultProperties.ContainsKey(property)) return this.blockType.DefaultProperties[property];
        throw new ArgumentException($"Property '{property}' does not exist for the block type: {this.blockType.Name}");
      }

      set
      {
        if (!this.blockType.DefaultProperties.ContainsKey(property))
          throw new ArgumentException($"Property '{property}' does not exist for the block type: {this.blockType.Name}");

        var defaultValue = this.blockType.DefaultProperties[property];
        if (value.Equals(defaultValue)) this.properties.Remove(property);
        else this.properties[property] = value;
      }
    }


    /// <summary>
    /// Creates a new block instance.
    /// </summary>
    /// <param name="chunk">The chunk this block is in.</param>
    /// <param name="pos">The position of this block in the world.</param>
    internal Block(IChunk chunk, BlockPos pos)
    {
      Chunk = chunk;
      Position = pos;
    }
  }
}
