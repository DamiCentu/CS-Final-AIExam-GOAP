using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode {
	public Vector3 position;
	public List<MapNode> adjacent = new List<MapNode>();
    public HashSet<Item> nearbyItems = new HashSet<Item>();
    public bool accessible = false;
    public float radius = .5f;

	public MapNode() {
	}

	public MapNode(float x, float y, float z) {
		position.Set(x, y, z);
	}

	public MapNode(Vector3 vec) {
		position = vec;
	}
}
