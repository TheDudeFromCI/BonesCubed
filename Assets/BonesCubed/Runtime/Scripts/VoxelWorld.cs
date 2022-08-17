using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Bones3.Jobs;
using Bones3.Native;

namespace Bones3.Runtime
{
  public class CustomWorld : World
  {
    protected override void CreateFields(Chunk chunk)
    {
      chunk.AddField<ushort>("model");
    }
  }


  public class VoxelWorld : MonoBehaviour
  {
    public Mesh grass;


    void Start()
    {
      var assets = new Bones3AssetReference();
      var grassId = assets.LoadBlockModel(grass);

      var world = new CustomWorld();
      var pos = new BlockPos(0, 0, 0);
      var chunk = world.GetChunk(pos, true);
      var field = chunk.GetField<ushort>("model");
      field[pos] = grassId;

      var chunkGrid = chunk.GetFieldAndSurrounding<ushort>("model");
      var meshData = new NativeMesh<VoxelVertex, ushort>(Allocator.TempJob);
      var remesh = new GenerateChunkMesh()
      {
        chunkData = chunkGrid,
        modelPointers = assets.ModelPointers,
        blockModelAtlas = assets.ModelAtlas,
        chunkMesh = meshData
      }.Schedule();
      remesh.Complete();

      var mesh = new Mesh();
      meshData.ApplyToMesh(mesh);
      meshData.Dispose();

      var go = new GameObject();
      go.AddComponent<MeshFilter>().sharedMesh = mesh;
      go.AddComponent<MeshRenderer>();

      world.Dispose();
      assets.Dispose();
    }
  }
}
