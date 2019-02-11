using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Navigation : MonoBehaviour
{
    public static Navigation instance;

    Map _map;

    private void Awake()
    {
        instance = this;
        _map = GetComponent<Map>();
        _map.Build();
    }

	public bool Reachable(Vector3 from, Vector3 to, List<Tuple<Vector3, Vector3>> debugRayList)
    {
		var srcWp = NearestTo(from);
		var dstWp = NearestTo(to);

		MapNode mapNode = srcWp;

		if(srcWp != dstWp) {
			var path = AStarNormal<MapNode>.Run(
				  srcWp
				, dstWp
				, (wa, wb) => Vector3.Distance(wa.position, wb.position)
				, w => w == dstWp
				, w =>
					w.adjacent
				//		.Where(a => a.nearbyItems.All(it => it.type != ItemType.Door))
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

	public IEnumerable<Item> AllInventories() {
		return AllItems()
			.Select(x => x.GetComponent<Entity>())
			.Where(x => x != null)
			.Aggregate(FList.Create<Item>(), (a, x) => a + x.items);
	}

	public IEnumerable<Item> AllItems() {
		return All().Aggregate(FList.Create<Item>(), (a, wp) => a += wp.nearbyItems);
	}

	public IEnumerable<MapNode> All() {
		return _map.GetNodes();
	}

	public MapNode Random() {
		return _map.GetRandomMapNodeAccesible();
	}

	public MapNode NearestTo(Vector3 pos) {
		return _map.FindClosestNode(pos);
	}
}
