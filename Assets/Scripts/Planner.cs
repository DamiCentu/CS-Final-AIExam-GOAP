using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Planner : MonoBehaviour {

	List<Tuple<Vector3, Vector3>> debugRayList = new List<Tuple<Vector3, Vector3>>();

	void Start ()
    {
		StartCoroutine(Plan());
	}

    //Check item availability
	void Check(Dictionary<string, bool> state, string name, ItemType type) {

		var items = Navigation.instance.AllItems();
		var inventories = Navigation.instance.AllInventories();
		var floorItems = items.Except(inventories);
		//Check if an item exists and is reachable.
		var item = floorItems.FirstOrDefault(x => x.type == type);
		var here = transform.position;
		state["accessible" + name] = item != null && Navigation.instance.Reachable(here, item.transform.position, debugRayList);

		var inv = inventories.Any(x => x.type == type);
		state["otherHas" + name] = inv;

		state["dead" + name] = false;
	}

	IEnumerator Plan() {
		yield return new WaitForSeconds(0.2f);

		//Process preconditions.
		var observedState = new Dictionary<string, bool>();
		
		//Consigo los items
		var nav = Navigation.instance;
		var floorItems = nav.AllItems();
		var inventory = nav.AllInventories();
		var everything = nav.AllItems().Union(nav.AllInventories());

        //Chequeo los booleanos para cada Item, generando mi modelo de mundo (mi diccionario de bools) en ObservedState
		Check(observedState, "Key"			, ItemType.Key);
		Check(observedState, "Other"		, ItemType.Entity);
		Check(observedState, "Mace"		    , ItemType.Mace);
        Check(observedState, "Mine", ItemType.Mine);
        Check(observedState, "PastaFrola"	, ItemType.PastaFrola);
		Check(observedState, "Door"	        , ItemType.Door);

		foreach(var kv in observedState.OrderBy(x => x.Key))
        {
			Debug.Log(string.Format("{0:12} : {1}", kv.Key, kv.Value));
		}

        List<GOAPAction> actions = CreatePossibleActionsList();

        GOAPState initial = new GOAPState();
		initial.boolValues = observedState; //le asigno los valores actuales, conseguidos antes
		initial.intValues["hasGold"] = 0; //agrego el bool "doorOpen"

        GOAPState goal = new GOAPState();
        goal.intValues["hasGold"] = 10;
        //goal.values["hasKey"] = true;
        //goal.boolValues["hasMace"] = true;

		var typeDict = new Dictionary<string, ItemType>() {
			  { "o", ItemType.Entity }
			, { "k", ItemType.Key }
			, { "d", ItemType.Door }
			, { "m", ItemType.Mace }
            , { "mine", ItemType.Mine }
            , { "pf", ItemType.PastaFrola }
		};
		var actDict = new Dictionary<string, TipitoAction>() {
			  { "Kill"	, TipitoAction.Kill }
			, { "Pickup", TipitoAction.PickUp }
			, { "Open"	, TipitoAction.Open }
		};

		var plan = GoapMiniTest.GoapRun(initial, goal, actions);

		if(plan == null)
			Debug.Log("Couldn't plan");
		else {
			GetComponent<Tipito>().ExecutePlan(
				plan
                .Select(pa => pa.name)
				.Select(a => 
                {
                    var i2 = everything.FirstOrDefault(i => typeDict.Any(kv => a.EndsWith(kv.Key)) ?
                                        i.type == typeDict.First(kv => a.EndsWith(kv.Key)).Value :
                                        false);
                    if (actDict.Any(kv => a.StartsWith(kv.Key)) && i2 != null)
                    {
                        return Tuple.Create(actDict.First(kv => a.StartsWith(kv.Key)).Value, i2);
                    }
                    else
                    {
                        return null;
                    }
				}).Where(a => a != null)
				.ToList()
			);
		}
	}

    private List<GOAPAction> CreatePossibleActionsList()
    {
        return new List<GOAPAction>()
        {
 /*             new GOAPAction("Kill o")
                .Cost(100f)
                .Pre("deadOther", false)
                .Pre("accessibleOther", true)
                .Pre("hasMace", true)

                .Effect("deadOther", true)

            , new GOAPAction("Loot k")
                .Cost(1f)
                .Pre("otherHasKey", true)
                .Pre("deadOther", true)

                .Effect("accessibleKey", true)
                .Effect("otherHasKey", false)

            , new GOAPAction("Pickup m")
                .Cost(2f)
                .Pre("deadMace", false)
                .Pre("otherHasMace", false)
                .Pre("accessibleMace", true)

                .Effect("accessibleMace", false)
                .Effect("hasMace", true)

            , new GOAPAction("Pickup k")
                .Cost(2f)
                .Pre("deadKey", false)
                .Pre("otherHasKey", false)
                .Pre("accessibleKey", true)

                .Effect("accessibleKey", false)
                .Effect("hasKey", true)

            , new GOAPAction("Pickup pf")
                .Cost(1f)					//La frola es prioritaria!
				.Pre("deadPastaFrola", false)
                .Pre("otherHasPastaFrola", false)
                .Pre("accessiblePastaFrola", true)

                .Effect("accessiblePastaFrola", false)
                .Effect("hasPastaFrola", true),*/

             new GOAPAction("Pickup mine")
                .Cost(1f)					//mine es prioritaria!
                .Effect((GOAPState state) => {
                    state.intValues["hasGold"] += 10;
                })

   /*         , new GOAPAction("Open d")
                .Cost(3f)
                .Pre("deadDoor", false)
                .Pre("hasKey", true)

                .Effect("hasKey", false)
                .Effect("doorOpen", true)
                .Effect("deadKey", true)
                .Effect("accessiblePastaFrola", true)

            , new GOAPAction("Kill d")
                .Cost(50f)
                .Pre("deadDoor", false)
                .Pre("hasMace", true)

                .Effect("doorOpen", true)
                .Effect("hasMace", false)
                .Effect("deadMace", true)
                .Effect("deadDoor", true)
                .Effect("accessibleKey", true)
                .Effect("accessiblePastaFrola", true)*/
        };
    }

    void OnDrawGizmos()
    {
		Gizmos.color = Color.cyan;
		foreach(var t in debugRayList)
        {
			Gizmos.DrawRay(t.Item1, (t.Item2-t.Item1).normalized);
			Gizmos.DrawCube(t.Item2+Vector3.up, Vector3.one*0.2f);
		}
	}
}
