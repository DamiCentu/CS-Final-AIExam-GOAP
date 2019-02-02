using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using IA2;

public enum TipitoAction
{
    Null,
    Stop,
    Run, 
    Kill,
    PickUp,
	NextStep,
	FailedStep,
	Open,
	Success,
    Create,
    Upgrade,
    Attack,
    Wait
}

public class Tipito : MonoBehaviour
{
    EventFSM<TipitoAction> _fsm;
	Item _target;
    bool insideItem = false;
	Entity _ent;

	IEnumerable<Tuple<TipitoAction, Item>> _plan;

	void PerformOpen(Entity us, Item other) {
		if(other != _target) return;
		Debug.Log("Open");
		
		var key = _ent.items.FirstOrDefault(it => it.type == ItemType.Key);
		var door = other.GetComponent<Door>();
		if(door && key) {
			door.Open();
			//Consume key
			Destroy(_ent.Removeitem(key).gameObject);
			_fsm.Feed(TipitoAction.NextStep);
		}
		else
			_fsm.Feed(TipitoAction.FailedStep);
	}

	void PerformPickUp(Entity us, Item other) {
		if(other != _target || insideItem)
            return;
		Debug.Log("Pickup");
        //	_ent.AddItem(other);
        //     if (other.type == ItemType.Mine) {
        //        other.gameObject.SetActive(false);
        //     }
        //	_fsm.Feed(TipitoAction.NextStep);
        insideItem = true;
        StartCoroutine(Wait(1.0f));
    }

    void PerformCreate(Entity us, Item other)
    {
        if (other != _target) return;
        Debug.Log("Create");
        //	_ent.AddItem(other);
        if (other.type == ItemType.Defense) {
            var defense = other.GetComponent<Defense>();
            defense.Create();
        }

        if (other.type == ItemType.Cannon)
        {
            var cannon = other.GetComponent<Cannon>();
            cannon.Create();
        }

        _fsm.Feed(TipitoAction.NextStep);
    }

    void PerformAttack(Entity us, Item other)
    {
        if (other != _target) return;
        Debug.Log("Attack");

        if (other.type == ItemType.Cannon)
        {
            var cannon = other.GetComponent<Cannon>();
            cannon.Attack();
        }
        _fsm.Feed(TipitoAction.NextStep);
    }

    private void PerformWait(Entity arg1, Item arg2)
    {
        if (arg2 != _target)
            return;
        StartCoroutine(Wait(1.0f));
    }

    IEnumerator Wait(float waitTime)
    {
        print("WaitForSecondse!");
        yield return new WaitForSeconds(waitTime);
        print("ya espere!");
        _fsm.Feed(TipitoAction.NextStep);
    }

    void PerformUpgrade(Entity us, Item other)
    {
        if (other != _target) return;
        Debug.Log("Upgrade");
        //	_ent.AddItem(other);
        if (other.type == ItemType.Defense)
        {
            var defense = other.GetComponent<Defense>();
            defense.Upgrade();
        }

        if (other.type == ItemType.Cannon)
        {
            var cannon = other.GetComponent<Cannon>();
            cannon.Upgrade();
        }
        _fsm.Feed(TipitoAction.NextStep);
    }

    void NextStep(Entity ent, Waypoint wp, bool reached) {
		Debug.Log("On reach target Next step");
		_fsm.Feed(TipitoAction.NextStep);
	}

    void Awake() {
		_ent = GetComponent<Entity>();

        var any = new State<TipitoAction>("any");
        var idle = new State<TipitoAction>("idle");
        var planStep = new State<TipitoAction>("planStep");
        var failStep = new State<TipitoAction>("failStep");
        var pickup = new State<TipitoAction>("pickup");
        var create = new State<TipitoAction>("create");
        var upgrade = new State<TipitoAction>("upgrade");
        var attack = new State<TipitoAction>("attack");
        var open = new State<TipitoAction>("open");
        var success = new State<TipitoAction>("success");
        var wait = new State<TipitoAction>("wait");


        failStep.OnEnter += a => { _ent.Stop(); Debug.Log("Plan failed"); };

		pickup.OnEnter += a => {
            Debug.Log("pickup.OnEnter");
            _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformPickUp;
            Debug.Log("pickup.OnEnter finish");
        };
        pickup.OnExit += a =>
        {

            Debug.Log("pickup.OnExit");
            _ent.OnHitItem -= PerformPickUp;
            insideItem = false;
            Debug.Log("pickup.OnExit finish");
        };

        wait.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformWait;
            print("entro en el wait.onEnter");
            print(_target);
        };
        wait.OnExit += a => { _ent.OnHitItem -= PerformWait;
        print("entro en el wait.onExit");
        };

        open.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformOpen; };
		open.OnExit += a => _ent.OnHitItem -= PerformOpen;

        create.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformCreate; };
        create.OnExit += a => _ent.OnHitItem -= PerformCreate;


        attack.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformAttack; };
        attack.OnExit += a => _ent.OnHitItem -= PerformAttack;


        upgrade.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformUpgrade; };
        upgrade.OnExit += a => _ent.OnHitItem -= PerformUpgrade;

        planStep.OnEnter += a => {
			Debug.Log("Plan next step");
			var step = _plan.FirstOrDefault();
			if(step != null) {
				Debug.Log("Next step:" + step.Item1 + "," + step.Item2);

				_plan = _plan.Skip(1);
				var oldTarget = _target;
				_target = step.Item2;
				if(!_fsm.Feed(step.Item1))
					_target = oldTarget;		//Revert if feed failed.
			}
			else
            {
                Debug.Log("TipitoAction.Success");
                _fsm.Feed(TipitoAction.Success);
			}
		};


        success.OnEnter += a => { Debug.Log("Success"); };
		success.OnUpdate += () => { _ent.Jump(); };
		
		StateConfigurer.Create(any)
			.SetTransition(TipitoAction.NextStep, planStep)
			.SetTransition(TipitoAction.FailedStep, idle)
			.Done();

        StateConfigurer.Create(planStep)
            .SetTransition(TipitoAction.PickUp, pickup)
            .SetTransition(TipitoAction.Create, create)
            .SetTransition(TipitoAction.Attack, attack)
            .SetTransition(TipitoAction.Upgrade, upgrade)
            .SetTransition(TipitoAction.Open, open)
            .SetTransition(TipitoAction.Success, success)
            .SetTransition(TipitoAction.Wait, wait)
			.Done();
        
		_fsm = new EventFSM<TipitoAction>(idle, any);
    }


    public void ExecutePlan(List<Tuple<TipitoAction, Item>> plan) {
		_plan = plan;
		_fsm.Feed(TipitoAction.NextStep);
	}

    void Update ()
    {
		//Never forget
        _fsm.Update();
	}
}
