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

    IABehaviour _ia;
    Entity _ent;
    bool aggressivePlan = true;

    void Start ()
    {
        _ia = GetComponent<IABehaviour>();
        if (!_ia)
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
        var everything = nav.AllItems(_ent.ownerType).Union(nav.AllInventories(_ent.ownerType));

        List<GOAPAction> actions = CreatePossibleActionsList();
        if (firstTime)
        {
            initial.worldSpace = new WorldSpace();
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
        var actDict = new Dictionary<string, IAAction>() {
                 { "Pickup", IAAction.PickUp }
                , { "Create", IAAction.Create }
                , { "Upgrade", IAAction.Upgrade }
                , { "Attack", IAAction.Attack }
                , { "Wait", IAAction.Wait }
                , { "SuperAttack", IAAction.SuperAttack }
            };

        var plan = GoapMiniTest.GoapRun(initial, goal, actions,true);

        if (plan == null)
            print("Couldn't plan");
        else
        {
            _ia.SetPlan(
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
                _ia.NextStep();

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

    public List<Tuple<IAAction, Item>>  RecalculatePlan(GOAPState curState) {
        var observedState = new Dictionary<string, bool>();

        var nav = Navigation.instance;
        var everything = nav.AllItems(_ent.ownerType).Union(nav.AllInventories(_ent.ownerType));

        List<GOAPAction> actions = CreatePossibleActionsList();

        GOAPState goal = new GOAPState();
        SetGoals(goal);

        var typeDict = new Dictionary<string, ItemType>() {

                 { "mine", ItemType.Mine }
                , { "defense", ItemType.Defense }
                , { "cannon", ItemType.Cannon }
                , { "core", ItemType.Core }
                , { "workTable", ItemType.WorkTable }
                , { "waitZone", ItemType.WaitZone }
            };
        var actDict = new Dictionary<string, IAAction>() {
                 { "Pickup", IAAction.PickUp }
                , { "Create", IAAction.Create }
                , { "Upgrade", IAAction.Upgrade }
                , { "Attack", IAAction.Attack }
                , { "Wait", IAAction.Wait }
                , { "SuperAttack", IAAction.SuperAttack }
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
            goal.worldSpace.enemyLife = 0;
            goal.worldSpace.cannon = "cannon";
        }
        else {
            goal.worldSpace.hasDefense = true;
            goal.worldSpace.defenseIsRepaired = true;
        }
    }

    private List<GOAPAction> CreatePossibleActionsList()
    {
        return new List<GOAPAction>()
        {
            new GOAPAction("Pickup mine")
            .Effect((WorldSpace w) =>  w.PickUpMine()),

            new GOAPAction("Create defense")
            .Pre((WorldSpace w) => w.CanCreateDefense())
            .Effect((WorldSpace w) =>  w.CreateDefense()),

             new GOAPAction("Create workTable")
            .Pre((WorldSpace w) => w.CanCreateWorktable())
            .Effect((WorldSpace w) =>  w.CreateWorktable()),

            new GOAPAction("SuperAttack cannon")
            .Pre((WorldSpace w) => w.CanSuperAttack())
            .Effect((WorldSpace w) =>  w.SuperAttack()),

            new GOAPAction("Attack cannon")
            .Pre((WorldSpace w) => w.CanAttack())
            .Effect((WorldSpace w) =>  w.Attack()),

            new GOAPAction("Create cannon")
            .Pre((WorldSpace w) => w.CanCreateCannon())
            .Effect((WorldSpace w) =>  w.CreateCannon()),

            new GOAPAction("Upgrade defense")
            .Pre((WorldSpace w) => w.CanUpgradedefense())
            .Effect((WorldSpace w) =>  w.Upgradedefense()),

            new GOAPAction("Upgrade cannon")
            .Pre((WorldSpace w) => w.CanUpgradeCannon())
            .Effect((WorldSpace w) =>  w.UpgradeCannon()),
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
