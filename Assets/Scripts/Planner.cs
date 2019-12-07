using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Planner : MonoBehaviour {

	List<Tuple<Vector3, Vector3>> debugRayList = new List<Tuple<Vector3, Vector3>>();
    private bool firstTime=true;

    GOAPState initial = new GOAPState();
    GOAPState cur = new GOAPState();

    Tipito _tipito;
    Entity _ent;
    bool aggressivePlan = true;

    void Start ()
    {
        _tipito = GetComponent<Tipito>();
        if (!_tipito)
            throw new Exception("Tipito null");

        _ent = GetComponent<Entity>();
        if (!_ent)
            throw new Exception("Entity null");

        StartPlan();
    }


    internal void StartPlan()
    {
        StartCoroutine("Plan");
    }


    IEnumerator Plan()
    {

        var observedState = new Dictionary<string, bool>();

        var nav = Navigation.instance;
        var floorItems = nav.AllItems(_ent.ownerType);
        var inventory = nav.AllInventories(_ent.ownerType);
        var everything = nav.AllItems(_ent.ownerType).Union(nav.AllInventories(_ent.ownerType));

        List<GOAPAction> actions = CreatePossibleActionsList();
        if (firstTime)
        {

            initial.intValues["hasGold"] = 0; 
            initial.boolValues["hasDefense"] = false;
            initial.boolValues["hasCannon"] = false;
            initial.boolValues["UpgradeCannon"] = false;
            initial.boolValues["UpgradeDefense"] = false;
            initial.floatValues["EnemyLife"] = 100f;
            initial.strignValues["bullet"] = "";
        }
        else { print("recalculo con los valore snuevos"); }

        GOAPState goal = new GOAPState();
        SetGoals(goal);

        ;

        StartPlannig(everything, actions, goal);
        yield return new WaitForSeconds(.1f);

    }

    private void StartPlannig(IEnumerable<Item> everything, List<GOAPAction> actions, GOAPState goal)
    {
        var typeDict = new Dictionary<string, ItemType>() {

                 { "mine", ItemType.Mine }
                , { "defense", ItemType.Defense }
                , { "cannon", ItemType.Cannon }
                , { "core", ItemType.Core }
                , { "workTable", ItemType.WorkTable }
                , { "waitZone", ItemType.WaitZone }
            };
        var actDict = new Dictionary<string, TipitoAction>() {
                 { "Pickup", TipitoAction.PickUp }
                , { "Create", TipitoAction.Create }
                , { "Upgrade", TipitoAction.Upgrade }
                , { "Attack", TipitoAction.Attack }
                , { "Wait", TipitoAction.Wait }
                , { "SuperAttack", TipitoAction.SuperAttack }
            };

        var plan = GoapMiniTest.GoapRun(initial, goal, actions,true);

        if (plan == null)
            print("Couldn't plan");
        else
        {
            _tipito.SetPlan(
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
            if (firstTime)
            {
                firstTime = false;
                _tipito.NextStep();

            }
        }
    }

    internal void SetAggressivePlan(bool v)
    {
        aggressivePlan = v;
    }
    internal void SetCurState(GOAPState curState)
    {
        cur = curState;
    }

    public List<Tuple<TipitoAction, Item>>  RecalculatePlan(GOAPState curState) {
        var observedState = new Dictionary<string, bool>();

        var nav = Navigation.instance;
        var floorItems = nav.AllItems(_ent.ownerType);
        var inventory = nav.AllInventories(_ent.ownerType);
        var everything = nav.AllItems(_ent.ownerType).Union(nav.AllInventories(_ent.ownerType));

        List<GOAPAction> actions = CreatePossibleActionsList();

        GOAPState goal = new GOAPState();

        SetGoals(goal);
        ;

        var typeDict = new Dictionary<string, ItemType>() {

                 { "mine", ItemType.Mine }
                , { "defense", ItemType.Defense }
                , { "cannon", ItemType.Cannon }
                , { "core", ItemType.Core }
                , { "workTable", ItemType.WorkTable }
                , { "waitZone", ItemType.WaitZone }
            };
        var actDict = new Dictionary<string, TipitoAction>() {
                 { "Pickup", TipitoAction.PickUp }
                , { "Create", TipitoAction.Create }
                , { "Upgrade", TipitoAction.Upgrade }
                , { "Attack", TipitoAction.Attack }
                , { "Wait", TipitoAction.Wait }
                , { "SuperAttack", TipitoAction.SuperAttack }
            };

        var plan = GoapMiniTest.GoapRun(curState, goal, actions,aggressivePlan?true:false);

        return plan
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
                .ToList();
    }

    private void SetGoals(GOAPState goal)
    {
        if (aggressivePlan)
        {
            goal.floatValues["EnemyLife"] = 0;
            goal.boolValues["hasCannon"] = true;
        }
        else {
            goal.boolValues["hasDefense"] = true;
            goal.boolValues["UpgradeDefense"] = true;
        }
    }

    private List<GOAPAction> CreatePossibleActionsList()
    {
        return new List<GOAPAction>()
        {
            new GOAPAction("Pickup mine")
            .Effect((GOAPState state) => {
                state.intValues["hasGold"] += 15;

            }),

            new GOAPAction("Create defense")
            .Pre((GOAPState state) => {
                return state.intValues["hasGold"] >= 20;
            })
            .Effect((GOAPState state) => {
                state.boolValues["hasDefense"] = true;
                state.intValues["hasGold"] -=20;
            }),
            new GOAPAction("Create workTable")
                .Pre((GOAPState state) => {
                    return state.strignValues["bullet"] == "" &&
                            state.intValues["hasGold"] >= 10;
                })
                .Effect((GOAPState state) => {
                    state.strignValues["bullet"] = "Normal Bullet";
                    state.intValues["hasGold"] -=10;
                }),

            new GOAPAction("SuperAttack cannon")
                .Pre((GOAPState state) => {
                    return state.strignValues["bullet"] == "Normal Bullet"  &&
                            state.boolValues["UpgradeCannon"] == true;
                })
                .Effect((GOAPState state) => {
                        state.strignValues["bullet"] = "";
                        state.floatValues["EnemyLife"] -= 40;
                }),

            new GOAPAction("Attack cannon")
                .Pre((GOAPState state) => {
                    return state.strignValues["bullet"] == "Normal Bullet"  &&
                            state.boolValues["hasCannon"] == true;
                })
                .Effect((GOAPState state) => {
                        state.strignValues["bullet"] = "";
                        state.floatValues["EnemyLife"] -= 25;
                }),

            new GOAPAction("Create cannon")
                .Pre((GOAPState state) => {
                    return state.intValues["hasGold"] >= 20;
                })
                .Effect((GOAPState state) => {
                    state.boolValues["hasCannon"] = true;
                    state.intValues["hasGold"] -=20;
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
                    return state.intValues["hasGold"] >= 40 &&
                    state.boolValues["hasCannon"] == true;
                })
                .Effect((GOAPState state) => {
                    state.boolValues["UpgradeCannon"] = true;
                    state.intValues["hasGold"] -=40;
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
