using System;
using System.Collections.Generic;
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
    [SerializeField]
    [Header("Model Variants")]
    [ReorderableList(ListStyle.Boxed, "Varient")]
    [Tooltip("An ordered list of available variants for this block model. The first variant that meets all conditions is used for the block model.")]
    private BlockModelVariant[] modelVariants;


    /// <inheritdoc/>
    public List<ModelVariant> GetVariants(List<Mesh> meshList)
    {
      var list = new List<ModelVariant>();

      foreach (var variant in this.modelVariants)
      {
        if (!meshList.Contains(variant.staticMesh)) meshList.Add(variant.staticMesh);

        var modelVariant = new ModelVariant();
        modelVariant.transform = variant.Transform;
        modelVariant.meshIndex = meshList.IndexOf(variant.staticMesh);
        list.Add(modelVariant);
      }

      return list;
    }
  }


  /// <summary>
  /// A Unity-friendly wrapper for a block model variant that can be modified
  /// from within the Unity editor.
  /// </summary>
  [Serializable]
  internal class BlockModelVariant
  {
    [NotNull]
    [SerializeField]
    [Tooltip("The mesh for this variant that is baked into the chunk mesh.")]
    internal Mesh staticMesh;


    [SerializeField]
    [Tooltip("The mesh translation to apply to this variant.")]
    internal Vector3 translation;


    [SerializeField]
    [Tooltip("The mesh rotation to apply to this variant.")]
    internal Vector3 rotation;


    [SerializeField]
    [Tooltip("The mesh scale to apply to this variant.")]
    internal Vector3 scale = Vector3.one;


    /// <summary>
    /// Computes the transformation matrix for this model variant.
    /// </summary>
    internal Matrix4x4 Transform => Matrix4x4.TRS(this.translation, Quaternion.Euler(this.rotation), this.scale);
  }
}
