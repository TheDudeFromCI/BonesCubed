using UnityEngine;

namespace Bones3.Runtime
{
  /// <summary>
  /// A Unity-friendly implementation of a serializable block type. This asset
  /// is intended to be used with Unity Addressables for access and use.
  /// </summary>
  [CreateAssetMenu(fileName = "New Block Model", menuName = "Bones3/Block Model")]
  public class BlockModel : ScriptableObject, IBlockModel
  {
    [Header("Properties")]
    [SerializeField]
    [Tooltip("The static mesh to use for for this block model.")]
    private Mesh mesh;


    [Space]
    [SerializeField]
    [EnumToggles]
    [Tooltip("The directions of this block model that block neighboring blocks.")]
    private IBlockModel.OccludingSegment occludingDirections;


    /// <inheritdoc/>
    public Mesh Mesh => this.mesh;


    /// <inheritdoc/>
    public IBlockModel.OccludingSegment OccludingDirections => this.occludingDirections;
  }
}
