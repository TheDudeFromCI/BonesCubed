using UnityEngine;
using Unity.Jobs;
using Bones3.Jobs;
using Unity.Collections;
using Bones3.Native;

namespace Bones3.Runtime
{
  public class CustomWorld : World
  {
    protected override void CreateFields(Chunk chunk)
    {
      chunk.AddField<BlockMeshData>("model");
    }
  }


  public class VoxelWorld : MonoBehaviour
  {
    public void Start()
    {
      var world = new CustomWorld();
      var pos = new BlockPos(0, 0, 0);
      var chunk = world.GetChunk(pos, true);
      var field = chunk.GetField<BlockMeshData>("model");
      field[pos] = new BlockMeshData() { IsSolid = true };

      var chunkGrid = chunk.GetFieldAndSurrounding<BlockMeshData>("model");

      var meshData = new NativeMesh<VoxelVertex, ushort>(Allocator.TempJob);
      var remesh = new GenerateChunkMesh()
      {
        chunkData = chunkGrid,
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
    }
  }
}
