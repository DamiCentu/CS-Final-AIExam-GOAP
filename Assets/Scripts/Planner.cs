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

        Check(observedState, "Mine", ItemType.Mine);
      //  Check(observedState, "PastaFrola"	, ItemType.PastaFrola);
		Check(observedState, "Defense", ItemType.Defense);
        Check(observedState, "Cannon", ItemType.Cannon);


        foreach (var kv in observedState.OrderBy(x => x.Key))
        {
	//		Debug.Log(string.Format("{0:12} : {1}", kv.Key, kv.Value));
		}

        List<GOAPAction> actions = CreatePossibleActionsList();

        GOAPState initial = new GOAPState();
		initial.boolValues = observedState; //le asigno los valores actuales, conseguidos antes
		initial.intValues["hasGold"] = 0; //agrego el bool "doorOpen"     //   initial.boolValues["hasDefense"] = false;
        initial.intValues["waitTime"] = 0;
        initial.boolValues["hasDefense"] = false;
        initial.boolValues["hasCannon"] = false;
        initial.boolValues["UpgradeCannon"] = false;
        initial.boolValues["UpgradeDefense"] = false;

        GOAPState goal = new GOAPState();
        goal.boolValues["UpgradeCannon"] = true;
        goal.boolValues["UpgradeDefense"] = true;
        ;
        //  goal.intValues["hasGold"] = 20;
        //goal.boolValues["hasMace"] = true;

        var typeDict = new Dictionary<string, ItemType>() {

             { "mine", ItemType.Mine }
            , { "defense", ItemType.Defense }
            , { "cannon", ItemType.Cannon }
            , { "core", ItemType.Core }
            , { "waitZone", ItemType.WaitZone }
        };
		var actDict = new Dictionary<string, TipitoAction>() {
			 { "Pickup", TipitoAction.PickUp }
            , { "Create", TipitoAction.Create }
            , { "Upgrade", TipitoAction.Upgrade }
            , { "Attack", TipitoAction.Attack }
            , { "Wait", TipitoAction.Wait }
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

             new GOAPAction("Pickup mine")
          //      .Pre((GOAPState state) => {
         //           return state.intValues["waitTime"] == 1;
      //          })
                .Effect((GOAPState state) => {
                    state.intValues["hasGold"] += 100;
         //           state.intValues["waitTime"] -=1;
                }),

      /*        new GOAPAction("Wait waitZone")
               					//mine es prioritaria!
                .Effect((GOAPState state) => {
                    state.intValues["waitTime"] += 1;
                }),
                */
             new GOAPAction("Create defense")				
                .Pre((GOAPState state) => {
                    return state.intValues["hasGold"] >= 20;
                })
                .Effect((GOAPState state) => {
                    state.boolValues["hasDefense"] = true;
                    state.intValues["hasGold"] -=20;
                }),

             new GOAPAction("Create cannon")				
                .Pre((GOAPState state) => {
                    return state.intValues["hasGold"] >= 30;
                })
                .Effect((GOAPState state) => {
                    state.boolValues["hasCannon"] = true;
                    state.intValues["hasGold"] -=30;
                }),

              new GOAPAction("Upgrade defense")
                .Pre((GOAPState state) => {
                    return state.intValues["hasGold"] >= 40 && 
                    state.boolValues["hasDefense"] == true;
                })
                .Effect((GOAPState state) => {
                    state.boolValues["UpgradeDefense"] = true;
                    state.intValues["hasGold"] -=40;
                }),

             new GOAPAction("Upgrade cannon")
                .Pre((GOAPState state) => {
                    return state.intValues["hasGold"] >= 60 &&
                    state.boolValues["hasCannon"] == true;
                })
                .Effect((GOAPState state) => {
                    state.boolValues["UpgradeCannon"] = true;
                    state.intValues["hasGold"] -=60;
                })
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
