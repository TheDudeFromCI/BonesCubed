using System;
using UnityEngine;

namespace Bones3.Runtime
{
  /// <summary>
  /// A condensed version of OccludingVoxelVertexSegement for editor readability
  /// purposes.
  /// </summary>
  [Flags]
  public enum OccludingDirections
  {
    Nothing = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8,
    Top = 16,
    Bottom = 32,
  }

  /// <summary>
  /// A Unity-friendly implementation of a serializable block model. This asset
  /// is intended to be used with Unity Addressables for access and use.
  /// </summary>
  [CreateAssetMenu(fileName = "New Block Model", menuName = "Bones3/Block Model")]
  public class BlockModel : ScriptableObject, IBlockModel
  {
    [SerializeField]
    [AssetPreview(128, 128)]
    [Tooltip("The static mesh to use for for this block model.")]
    private Mesh staticMesh;

    [SerializeField]
    [EnumToggles]
    [Tooltip("The directions of this block model that block neighboring blocks.")]
    private OccludingDirections occludingDirections;


    /// <inheritdoc/>
    public Mesh StaticMesh => this.staticMesh;


    /// <inheritdoc/>
    public OccludingVoxelVertexSegement OccludingDirections => ((OccludingVoxelVertexSegement)this.occludingDirections);
  }
}
