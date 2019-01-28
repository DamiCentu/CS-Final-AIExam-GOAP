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
	Success
}

public class Tipito : MonoBehaviour
{
    EventFSM<TipitoAction> _fsm;
	Item _target;

	Entity _ent;

	IEnumerable<Tuple<TipitoAction, Item>> _plan;

	void PerformAttack(Entity us, Item other) {
		if(other != _target) return;
		Debug.Log("Attack");
		
		var mace = _ent.items.FirstOrDefault(it => it.type == ItemType.Mace);
		if(mace) {
			other.Kill();
			if(other.type == ItemType.Door)
				Destroy(_ent.Removeitem(mace).gameObject);
			_fsm.Feed(TipitoAction.NextStep);
		}
		else
			_fsm.Feed(TipitoAction.FailedStep);
	}

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
		if(other != _target) return;
		Debug.Log("Pickup");
		_ent.AddItem(other);
        if (other.type == ItemType.Mine) {
            other.gameObject.SetActive(false);
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
        var kill = new State<TipitoAction>("kill");
        var pickup = new State<TipitoAction>("pickup");
        var open = new State<TipitoAction>("open");
        var success = new State<TipitoAction>("success");

		kill.OnEnter += a => {
			_ent.GoTo(_target.transform.position);
			_ent.OnHitItem += PerformAttack;
		};

		kill.OnExit += a => _ent.OnHitItem -= PerformAttack;

		failStep.OnEnter += a => { _ent.Stop(); Debug.Log("Plan failed"); };

		pickup.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformPickUp; };
		pickup.OnExit += a => _ent.OnHitItem -= PerformPickUp;

		open.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformOpen; };
		open.OnExit += a => _ent.OnHitItem -= PerformOpen;

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
			else {
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
            .SetTransition(TipitoAction.Kill, kill)
            .SetTransition(TipitoAction.PickUp, pickup)
            .SetTransition(TipitoAction.Open, open)
            .SetTransition(TipitoAction.Success, success)
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
