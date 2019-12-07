using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Navigation : MonoBehaviour
{
    public const string PLAYER = "player";
    public const string IA = "ia";
    public static Navigation instance;

    Dictionary<string,Map> _maps = new Dictionary<string,Map>();

    private void Awake()
    {
        instance = this;
        _maps = GetComponentsInChildren<Map>().ToDictionary(x => x.owner, x => x );
        if (_maps == null)
            throw new Exception("Maps null");
        _maps[PLAYER].Build();
        _maps[IA].Build();
    }

    public bool Reachable(Vector3 from, Vector3 to, List<Tuple<Vector3, Vector3>> debugRayList, string owner)
    {
		var srcWp = NearestTo(from, owner);
		var dstWp = NearestTo(to, owner);

		MapNode mapNode = srcWp;

		if(srcWp != dstWp) {
			var path = AStarNormal<MapNode>.Run(
				  srcWp
				, dstWp
				, (wa, wb) => Vector3.Distance(wa.position, wb.position)
				, w => w == dstWp
				, w =>
					w.adjacent
						.Select(a => new AStarNormal<MapNode>.Arc(a, Vector3.Distance(a.position, w.position)))
			);
			if(path == null)
				return false;

			mapNode = path.Last();
		}
	//	Debug.Log("Reachable from " + wp.name);
		if(debugRayList != null) debugRayList.Add(Tuple.Create(mapNode.position, to));

		var delta = (to - mapNode.position);
		var distance = delta.magnitude;

		return !Physics.Raycast(mapNode.position, delta/distance, distance, LayerMask.GetMask(new []{"Blocking"}));
	}

	public IEnumerable<Item> AllInventories(string owner) {
		return AllItems(owner)
			.Select(x => x.GetComponent<Entity>())
			.Where(x => x != null)
			.Aggregate(FList.Create<Item>(), (a, x) => a + x.items);
	}

	public IEnumerable<Item> AllItems(string owner) {
		return AllMapNodes(owner).Aggregate(FList.Create<Item>(), (a, wp) => a += wp.nearbyItems);
	}

	public IEnumerable<MapNode> AllMapNodes(string owner) {
		return _maps[owner] != null ? _maps[owner].GetNodes() : Enumerable.Empty<MapNode>();
	}

	public MapNode Random(string owner) {
		return _maps[owner] != null ? _maps[owner].GetRandomMapNodeAccesible() : null; 
	}

	public MapNode NearestTo(Vector3 pos, string owner) {
		return _maps[owner] != null ? _maps[owner].FindClosestNode(pos) : null;
	}
}
