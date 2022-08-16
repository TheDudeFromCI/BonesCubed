namespace Bones3
{
  public struct BlockMeshData
  {
    private byte isSolid;


    /// <summary>
    /// Gets or sets whether or not this block mesh data is a solid block. If
    /// false, this block is considered invisible. If true, this block is
    /// considered a cube.
    /// </summary>
    public bool IsSolid
    {
      get => this.isSolid > 0;
      set => this.isSolid = value ? (byte)1 : (byte)0;
    }
  }
}
