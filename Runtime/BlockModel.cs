using UnityEngine;
using LibSugar;

namespace Bones3.Runtime
{
  /// <summary>
  /// A Unity-friendly implementation of a serializable block model. This asset
  /// is intended to be used with Unity Addressables for access and use.
  /// </summary>
  [CreateAssetMenu(fileName = "New Block Model", menuName = "Bones3/Block Model")]
  public class BlockModel : ScriptableObject, IBlockModel
  {
    [SerializeField]
    [Header("Model Variants")]
    [ReorderableList(ListStyle.Boxed, "Varient")]
    [Tooltip("An ordered list of available variants for this block model. The first variant that meets all conditions is used for the block model.")]
    private BlockModelVariant[] modelVariants;


    /// <inheritdoc/>
    public Option<IBlockModelVariant> GetVariant(IBlock block)
    {
      for (int i = 0; i < this.modelVariants.Length; i++)
        if (this.modelVariants[i].IsValidFor(block)) return Option<IBlockModelVariant>.Some(this.modelVariants[i]);

      return Option<IBlockModelVariant>.None;
    }
  }
}
