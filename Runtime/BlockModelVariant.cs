using System;
using UnityEngine;

namespace Bones3.Runtime
{
  [Serializable]
  public class BlockModelVariant : IBlockModelVariant
  {
    [NotNull]
    [SerializeField]
    [Tooltip("The mesh for this variant that is baked into the chunk mesh.")]
    private Mesh staticMesh;


    /// <inheritdoc/>
    public Mesh StaticMesh { get => this.staticMesh; }


    /// <inheritdoc/>
    public bool IsValidFor(IBlock block)
    {
      return true;
    }
  }
}
