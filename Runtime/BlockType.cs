using UnityEngine;
using System.Collections.ObjectModel;

namespace Bones3.Runtime
{
  /// <summary>
  /// A Unity-friendly implementation of a serializable block type. This asset
  /// is intended to be used with Unity Addressables for access and use.
  /// </summary>
  [CreateAssetMenu(fileName = "New Block Type", menuName = "Bones3/Block Type")]
  public class BlockType : ScriptableObject, IBlockType
  {
    private ReadOnlyDictionary<string, string> propertiesReadOnly;


    [BeginGroup("Metadata")]
    [BeginIndent]


    [SerializeField]
    [Tooltip("The name of this block type.")]
    [RegexValue(@".+", "Name cannot be empty!")]
    private new string name = "Unnamed Block Type";


    [SerializeField]
    [Tooltip("Gets whether or not this block type has a renderable, static mesh.")]
    private bool hasStaticMesh = true;


    [EndIndent]
    [EndGroup]
    [SpaceArea(10)]


    [SerializeField]
    [Header("Properties")]
    [Tooltip("A list of available properties for this block type and their default values.")]
    private SerializedDictionary<string, string> properties = new SerializedDictionary<string, string>();


    /// <inheritdoc/>
    public string Name { get => this.name; }


    /// <inheritdoc/>
    public bool HasStaticMesh { get => this.hasStaticMesh; }


    /// <inheritdoc/>
    public ReadOnlyDictionary<string, string> DefaultProperties
    {
      get
      {
        if (this.propertiesReadOnly == null) this.propertiesReadOnly = new ReadOnlyDictionary<string, string>(this.properties);
        return this.propertiesReadOnly;
      }
    }
  }
}
