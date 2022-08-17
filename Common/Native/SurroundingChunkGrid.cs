using System;

namespace Bones3.Native
{
  /// <summary>
  /// A utility wrapper for storing references to a 3x3x3 grid of chunk fields.
  /// This is useful for when a chunk processing job needs additional info from
  /// nearby chunks.
  /// </summary>
  /// <typeparam name="T">The type of data being stored in the chunk fields.</typeparam>
  public struct SurroundingChunkGrid<T> where T : struct
  {
    public NativeGrid3D<T> c0;
    public NativeGrid3D<T> c1;
    public NativeGrid3D<T> c2;
    public NativeGrid3D<T> c3;
    public NativeGrid3D<T> c4;
    public NativeGrid3D<T> c5;
    public NativeGrid3D<T> c6;
    public NativeGrid3D<T> c7;
    public NativeGrid3D<T> c8;
    public NativeGrid3D<T> c9;
    public NativeGrid3D<T> c10;
    public NativeGrid3D<T> c11;
    public NativeGrid3D<T> c12;
    public NativeGrid3D<T> c13;
    public NativeGrid3D<T> c14;
    public NativeGrid3D<T> c15;
    public NativeGrid3D<T> c16;
    public NativeGrid3D<T> c17;
    public NativeGrid3D<T> c18;
    public NativeGrid3D<T> c19;
    public NativeGrid3D<T> c20;
    public NativeGrid3D<T> c21;
    public NativeGrid3D<T> c22;
    public NativeGrid3D<T> c23;
    public NativeGrid3D<T> c24;
    public NativeGrid3D<T> c25;
    public NativeGrid3D<T> c26;


    /// <summary>
    /// Gets the native grid at the specified local block position.
    /// </summary>
    /// <param name="pos">The local block position.</param>
    /// <returns>The native grid for that block position.</returns>
    public NativeGrid3D<T> this[BlockPos pos]
    {
      get
      {
        pos = (pos >> 4) + 1;
        int chunkIndex = pos.z * 3 * 3 + pos.y * 3 + pos.x;
        return this[chunkIndex];
      }

      set
      {
        pos = (pos >> 4) + 1;
        int chunkIndex = pos.z * 3 * 3 + pos.y * 3 + pos.x;
        this[chunkIndex] = value;
      }
    }


    /// <summary>
    /// Gets the native grid within this struct at the specified index.
    /// </summary>
    /// <param name="index">The chunk index.</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException">If the index < 0 or > 26.</exception>
    public NativeGrid3D<T> this[int index]
    {
      get
      {
        switch (index)
        {
          case 0: return this.c0;
          case 1: return this.c1;
          case 2: return this.c2;
          case 3: return this.c3;
          case 4: return this.c4;
          case 5: return this.c5;
          case 6: return this.c6;
          case 7: return this.c7;
          case 8: return this.c8;
          case 9: return this.c9;
          case 10: return this.c10;
          case 11: return this.c11;
          case 12: return this.c12;
          case 13: return this.c13;
          case 14: return this.c14;
          case 15: return this.c15;
          case 16: return this.c16;
          case 17: return this.c17;
          case 18: return this.c18;
          case 19: return this.c19;
          case 20: return this.c20;
          case 21: return this.c21;
          case 22: return this.c22;
          case 23: return this.c23;
          case 24: return this.c24;
          case 25: return this.c25;
          case 26: return this.c26;
          default: throw new IndexOutOfRangeException($"Invalid index: {index}!");
        }
      }

      set
      {
        switch (index)
        {
          case 0: this.c0 = value; break;
          case 1: this.c1 = value; break;
          case 2: this.c2 = value; break;
          case 3: this.c3 = value; break;
          case 4: this.c4 = value; break;
          case 5: this.c5 = value; break;
          case 6: this.c6 = value; break;
          case 7: this.c7 = value; break;
          case 8: this.c8 = value; break;
          case 9: this.c9 = value; break;
          case 10: this.c10 = value; break;
          case 11: this.c11 = value; break;
          case 12: this.c12 = value; break;
          case 13: this.c13 = value; break;
          case 14: this.c14 = value; break;
          case 15: this.c15 = value; break;
          case 16: this.c16 = value; break;
          case 17: this.c17 = value; break;
          case 18: this.c18 = value; break;
          case 19: this.c19 = value; break;
          case 20: this.c20 = value; break;
          case 21: this.c21 = value; break;
          case 22: this.c22 = value; break;
          case 23: this.c23 = value; break;
          case 24: this.c24 = value; break;
          case 25: this.c25 = value; break;
          case 26: this.c26 = value; break;
          default: throw new IndexOutOfRangeException($"Invalid index: {index}!");
        }
      }
    }


    /// <summary>
    /// Gets the block value within this 3x3x3 chunk grid based off the local
    /// block coordinates of the center chunk.
    /// </summary>
    /// <param name="pos">The local block position.</param>
    /// <returns>The data value at the given location.</returns>
    public T GetLocalBlock(BlockPos pos)
    {
      return this[pos][pos & 15];
    }
  }
}
