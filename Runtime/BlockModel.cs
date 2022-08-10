using System;
using UnityEngine;

namespace Bones3.Runtime
{
  /// <summary>
  /// A Unity-friendly implementation of a serializable block model. This asset
  /// is intended to be used with Unity Addressables for access and use.
  /// </summary>
  [CreateAssetMenu(fileName = "New Block Model", menuName = "Bones3/Block Model")]
  public class BlockModel : ScriptableObject, IBlockModel
  {
    [Serializable]
    struct VariantProperties
    {
      [SerializeField]
      [ReorderableList]
      [Header("Required Properties")]
      [Tooltip("A list of available properties for this block type and their default values.")]
      public SerializedDictionary<string, string> properties;

      string DisplayName { get => properties.ToString(); }
    }

    [SerializeField]
    [Header("Model Variants")]
    [ReorderableList(ListStyle.Boxed, "Varient")]
    [Tooltip("A list of available properties for this block type and their default values.")]
    private SerializedDictionary<VariantProperties, Mesh> modelVariants = new SerializedDictionary<VariantProperties, Mesh>();
  }
}
