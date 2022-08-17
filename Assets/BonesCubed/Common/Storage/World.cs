using System;
using System.Collections.Generic;

namespace Bones3
{
  /// <summary>
  /// An infinite grid of blocks stored within chunks.
  /// </summary>
  public abstract class World : IDisposable
  {
    private readonly Dictionary<BlockPos, Chunk> chunks = new Dictionary<BlockPos, Chunk>();
    private Chunk voidChunk;


    /// <summary>
    /// Gets a reference to the void chunk object. This is used for storing data
    /// for chunks that have not yet been loaded. This chunk does not have a
    /// valid position and exists only as a reference object.
    /// </summary>
    public Chunk VoidChunk => this.voidChunk;


    /// <summary>
    /// Creates a new world instance and initializes default data.
    /// </summary>
    public World()
    {
      this.voidChunk = new Chunk(this, default);
      CreateFieldsVoid(this.voidChunk);
    }


    /// <summary>
    /// Gets the chunk that contains the given block coordinate.
    /// </summary>
    /// <param name="pos">The block position.</param>
    /// <param name="create">Whether or not to create the chunk if it does not currently exist.</param>
    /// <returns>The chunk, or null if it does not exist or could not be created.</returns>
    public Chunk GetChunk(BlockPos pos, bool create)
    {
      pos &= ~15;

      if (this.chunks.ContainsKey(pos)) return this.chunks[pos];
      if (!create) return null;

      var chunk = new Chunk(this, pos);
      this.chunks[pos] = chunk;
      CreateFields(chunk);

      return chunk;
    }


    /// <summary>
    /// Disposes and clears all chunks within this world, including the void
    /// chunk. The world cannot be used safely after this.
    /// </summary>
    public void Dispose()
    {
      foreach (var chunk in this.chunks.Values)
        chunk.Dispose();

      this.chunks.Clear();
      this.voidChunk.Dispose();
    }


    /// <summary>
    /// This method is called once when the world is first created in order to
    /// create and initialize the fields that exist within the "void". A void
    /// chunk is a chunk that is ungenerated or unloaded, and contains the
    /// fields that are returned when requesting data from chunks that do not
    /// exist. By default, this method simply calls the standard CreateFields()
    /// method and leaves all data at it's default value. This method can be
    /// overriden in order to assign a different default value for void chunks.
    /// </summary>
    /// <param name="chunk">The void chunk template.</param>
    protected virtual void CreateFieldsVoid(Chunk chunk)
    {
      CreateFields(chunk);
    }


    /// <summary>
    /// Creates all fields that should exist within this chunk. This is called
    /// once per chunk when that chunk is created or loaded. If a chunk is
    /// unloaded and then loaded again later, this method is called for the new
    /// chunk instance.
    /// </summary>
    /// <param name="chunk">The new chunk.</param>
    protected abstract void CreateFields(Chunk chunk);
  }
}
