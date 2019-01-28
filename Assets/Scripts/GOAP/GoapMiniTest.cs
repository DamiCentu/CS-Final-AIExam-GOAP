using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/*
Modo psicopata = Menos costo matar
	Se puede "patchear" in game!
*/

public class GoapMiniTest : MonoBehaviour
{
	public static IEnumerable<GOAPAction> GoapRun(GOAPState from, GOAPState to, IEnumerable<GOAPAction> actions)
    {
        int watchdog = 200;

        var seq = AStarNormal<GOAPState>.Run(
            from,
            to,
            (curr, goal) => goal.boolValues.Count(kv => !kv.In(curr.boolValues))+
                            goal.intValues.Count(i=> !i.In(curr.intValues)),
            curr => to.boolValues.All(kv => kv.In(curr.boolValues))&&
                    to.intValues.All(i => i.In(curr.intValues)),
            curr =>
            {
                if (watchdog == 0)
                    return Enumerable.Empty<AStarNormal<GOAPState>.Arc>();
                else
                    watchdog--;

                return actions.Where(action => action.preconditionsBool.All(kv => kv.In(curr.boolValues))&& 
                               action.preconditionsInt.All(k=> k.In(curr.intValues)))
                              .Aggregate(new FList<AStarNormal<GOAPState>.Arc>(), (possibleList, action) =>
                              {
                                  var newState = new GOAPState(curr);
                                  newState.boolValues.UpdateWith(action.effectsBool);
                                  newState.intValues.UpdateWith(action.effectsInt);
                                  newState.generatingAction = action;
                                  newState.step = curr.step+1;
                                  return possibleList + new AStarNormal<GOAPState>.Arc(newState, action.cost);
                              });
            });

        if (seq == null)
        {
            Debug.Log("Imposible planear");
            return null;
        }

        foreach (var act in seq.Skip(1))
        {
			Debug.Log(act);
        }

		Debug.Log("WATCHDOG " + watchdog);
		
		return seq.Skip(1).Select(x => x.generatingAction);
	}

    void Start()
    {
        /*var actions = new List<GOAPAction>() {
            new GOAPAction("BuildHouse")
                .Pre("hasWood", true)
                .Pre("hasHammer", true)
                .Effect("houseBuilt", true),

            new GOAPAction("CollectHammer")
                .Effect("hasHammer", true),

            new GOAPAction("CollectAxe")
                .Effect("hasAxe", true)
                .Cost(10f),

            new GOAPAction("CollectCheapAxe")
                .Effect("hasAxe", true)
                .Effect("backPain", true)
                .Cost(2f),

            new GOAPAction("ChopWood")
                .Pre("hasAxe", true)
                .Effect("hasWood", true),

            new GOAPAction("UseMedicine")
                .Effect("backPain", false)
                .Cost(100f),

            new GOAPAction("MineGold")
                .Effect("getGold", true),
        };
        var from = new GOAPState();
		from.boolValues["backPain"] = false;

        var to = new GOAPState();
        to.boolValues["houseBuilt"] = true;
        to.boolValues["backPain"] = false;

        GoapRun(from, to, actions);*/
	}
}
