using UnityEngine;

public interface IRenderable
{
	// People, Demand, Profession, (and mayeb Criterion?) implement UIRenderable
	// A RenderGroup (or just a function) updates renderables
	// The RenderGroup can also manage parent / position / visibility and know about the relevant UI object prefab

	// Updates properties of a UI object
	void RenderTo(GameObject uiPrefab);
}
