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
            (curr, goal) => curr.floatValues["EnemyLife"]- goal.floatValues["EnemyLife"],

            curr => to.boolValues.All(kv => kv.In(curr.boolValues))&&
                    to.intValues.All(i => i.In(curr.intValues)) &&
                    to.floatValues.All(i => i.In(curr.floatValues)) &&
                    to.strignValues.All(i => i.In(curr.strignValues)) ,

            curr =>
            {
                if (watchdog == 0)
                    return Enumerable.Empty<AStarNormal<GOAPState>.Arc>();
                else
                    watchdog--;

                //return actions.Where(action => action.preconditionsBool.All(kv => kv.In(curr.boolValues)) && 
                              // action.preconditionsInt.All(k=> k.In(curr.intValues)))
                return actions.Where(action => action.preConditions.All(f => f(curr) == true))
                             .Aggregate(new FList<AStarNormal<GOAPState>.Arc>(), (possibleList, action) =>
                              {
                                  var newState = new GOAPState(curr);
                              //    newState.boolValues.UpdateWith(action.effectsBool);
                              //    newState.intValues.UpdateWith(action.effectsInt);
                                  action.effects.ForEach((f) =>
                                  {
                                      f(newState);
                                  });
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

}
