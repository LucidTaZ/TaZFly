using UnityEngine;

/**
 * Chunk creation "pass"
 *
 * Each module is executed in turn to populate a new chunk with content. The order depends on how the ChunkGenerator
 * object is configured. For example, the terrain comes before objects, since objects need to snap to the ground.
 */
public interface IChunkCreationModule
{
	void AddChunkContents (GameObject chunk, ChunkCreationContext context);
	void SetChunkRegistry (ChunkRegistry registry);
}
