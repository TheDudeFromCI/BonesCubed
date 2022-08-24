using UnityEngine;

namespace Bones3.Runtime
{
  /// <summary>
  /// A Unity-friendly implementation of a serializable block type. This asset
  /// is intended to be used with Unity Addressables for access and use.
  /// </summary>
  [CreateAssetMenu(fileName = "New Block Type", menuName = "Bones3/Block Type")]
  public class BlockType : ScriptableObject, IBlockType
  {
    [SerializeField]
    [Tooltip("The name of this block type.")]
    [RegexValue(@".+", "Name cannot be empty!")]
    private new string name = "Unnamed Block Type";


    [SerializeField]
    [Tooltip("The block model to use for this block type.")]
    private BlockModel blockModel;


    [SerializeField]
    [Tooltip("The material to use for this block type.")]
    private Material material;


    /// <inheritdoc/>
    public string Name => this.name;


    /// <inheritdoc/>
    public Material Material => this.material;


    /// <inheritdoc/>
    public IBlockModel BlockModel => this.blockModel;
  }
}
