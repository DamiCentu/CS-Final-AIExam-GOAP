using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using IA2;

public enum IAAction
{
    Null,
    Stop,
    Run, 
    PickUp,
	NextStep,
	FailedStep,
	Open,
	Success,
    Create,
    Upgrade,
    Attack,
    Wait,
    SuperAttack
}

public class IABehaviour : PlayerAndIABaseBehaviour
{
    EventFSM<IAAction> _fsm;
	Item _target;
    bool action_started = false;
	Entity _ent;
    public bool shouldRePlan=false;
    public float waitTime=2f;
    private bool planAgresive = true;

    IEnumerable<Tuple<IAAction, Item>> _plan;
    private int _gold=0;
    private int bullets = 0;
    private int bullets_upgraded=0;
    private Dictionary<string, bool> structures = new Dictionary<string, bool>();
    private int count=0;

    public Core playerCore;

    public void Start()
    {
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_CREATE, PlayerCreateItem);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_UPGRADE, PlayerUpdateDefense);
        EventsManager.SubscribeToEvent(EventsConstants.IA_IS_BEING_ATTACK, IALifeChange);
        EventsManager.SubscribeToEvent(EventsConstants.IA_GET_DEFENSE, IALifeChange);

        structures["Cannon"] = false;
        structures["Defense"] = false;
        structures["CannonUpgraded"] = false;
        structures["DefenseUpgraded"] = false; 
    }

    private void PlayerUpdateDefense(object[] parameterContainer)
    {
        ItemType item = (ItemType)parameterContainer[0];
        if (item == ItemType.Defense)
        {
            shouldRePlan = true;
        }
    }

    private void StartAgressivePlan()
    {
        if (!planAgresive) {
            planAgresive = true;
            Planner p = GetComponent<Planner>();
            p.SetAggressivePlan(true);
        }
         shouldRePlan = true;
    }

    private void StartDefensivePlan()
    {
        if (planAgresive)
        {
            planAgresive = false;
            Planner p = GetComponent<Planner>();
            p.SetAggressivePlan(false);
        }
        shouldRePlan = true;
    }

    private void IALifeChange(object[] parameterContainer)
    {
        int life = (int)parameterContainer[0];
        if (life <= 75)
        {
            StartDefensivePlan();
            print("me pongo en plan defensivo");
        }
        else
        {
            print("me pongo en plan agresivo");
            StartAgressivePlan();
        }
    }

    private void PlayerCreateItem(object[] parameterContainer)
    {
        ItemType item = (ItemType)parameterContainer[0];
        if (item == ItemType.Defense)
        {
            StartAgressivePlan();
        }

        else if (item == ItemType.Cannon)
        {
            StartDefensivePlan();
        }
    }

    protected override void PerformPickUp(Entity ent, Item item) {
		if(item != _target || action_started)
            return;
		Debug.Log("Pickup");

        // deberia fallar el plan si me estan atacando y tengo suficiente oro como para hacer una defensa o ir a replan
        _gold += item.miningGoldValueGiven;

        EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD_IA, new object[] { _gold });
        EventsManager.TriggerEvent(EventsConstants.IA_MINNING);

        action_started = true;
        StartCoroutine(Wait(waitTime));
    }

    protected override void PerformCreate(Entity ent, Item item)
    {
        if (item != _target || action_started) return;
        Debug.Log("Create");
        action_started = true;
        if (_gold < item.itemCostToCreate) throw new Exception("no hay suficiente oro para crear el item, hay que replanear");// deberia re planear
        _gold -= item.itemCostToCreate;
        EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD_IA, new object[] { _gold });//ToDO No se actualiza el gold bien


        if (item.type == ItemType.Defense) {
            var defense = item.GetComponent<Defense>();
            defense.Create();
            defense.Create();
            structures["Defense"] = true;
            EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD_IA, new object[] { _gold });
        }

        if (item.type == ItemType.Cannon)
        {
            EventsManager.TriggerEvent(EventsConstants.IA_CREATE_CANNON);
            var cannon = item.GetComponent<Cannon>();
            cannon.Activate();
            structures["Cannon"] = true;
            EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD_IA, new object[] { _gold });
        }


        if (item.type == ItemType.WorkTable)
        {
            EventsManager.TriggerEvent(EventsConstants.IA_CREATE_BULLET);
            var workTable = item.GetComponent<WorkTable>();
            workTable.CreateBullet();
            bullets++;
            EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_BULLETS_IA, new object[] { bullets });
            EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD_IA, new object[] { _gold });
        }


        StartCoroutine(Wait(waitTime));
    }

    protected override void PerformAttack(Entity us, Item item)
    {
        if (item != _target||action_started) return;
        Debug.Log("Attack");
        action_started = true;
        if (item.type == ItemType.Cannon)
        {
            var cannon = item.GetComponent<Cannon>();
            if (bullets > 0)
            {
                if (structures["Cannon"]) cannon.AttackNormal();
                else  throw new Exception("no hay ninguna estructura para disparar, hay que replanear");// deberia re planear}
                bullets--;
                EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_BULLETS_IA, new object[] { bullets });
                EventsManager.TriggerEvent(EventsConstants.IA_SHOOTING);
            }
            else {
                throw new Exception("no hay suficientes balas para disparar, hay que replanear");// deberia re planear
            }
        }
        StartCoroutine(Wait(waitTime));
    }

    protected override void PerformSuperAttack(Entity us, Item item)
    {
        if (item != _target || action_started) return;
        Debug.Log("SuperAttack");
        action_started = true;
        if (item.type == ItemType.Cannon)
        {
            var cannon = item.GetComponent<Cannon>();
            if (bullets > 0)
            {
                if (structures["CannonUpgraded"]) cannon.AttackSpecial();
                else throw new Exception("no hay ninguna estructura actualizada para disparar, hay que replanear");// deberia re planear}
                bullets--;
                EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_BULLETS_IA, new object[] { bullets });
                EventsManager.TriggerEvent(EventsConstants.IA_SHOOTING);
            }
            else
            {
                throw new Exception("no hay suficientes balas para disparar, hay que replanear");// deberia re planear
            }
        }
        StartCoroutine(Wait(waitTime));
    }

    protected override void PerformWait(Entity ent, Item item)
    {
        if (item != _target)
            return;
        StartCoroutine(Wait(waitTime));
    }

    IEnumerator Wait(float waitTime)
    {
        print("WaitForSecondse!");
        yield return new WaitForSeconds(waitTime);
        print("ya espere!");

        NextStep();
        //_fsm.Feed(TipitoAction.NextStep);

    }

    protected override void PerformUpgrade(Entity ent, Item item)
    {
        if (item != _target|| action_started) return;
        Debug.Log("Upgrade");
        action_started = true;
        if (_gold < item.itemCostToUpgrade) throw new Exception("no hay suficiente oro para mejorar el item, hay que replanear");// deberia re planear
        _gold -= item.itemCostToCreate;
        EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD_IA, new object[] { _gold });

        //	_ent.AddItem(other);
        if (item.type == ItemType.Defense && structures["Defense"])
        {
            var defense = item.GetComponent<Defense>();
            defense.Upgrade();
            EventsManager.TriggerEvent(EventsConstants.IA_UPGRADE_DEFENSE);
            structures["DefenseUpgraded"] = true;
            EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD_IA, new object[] { _gold });
        }

        else if (item.type == ItemType.Cannon && structures["Cannon"])
        {
            var cannon = item.GetComponent<Cannon>();
            cannon.Upgrade();
            EventsManager.TriggerEvent(EventsConstants.IA_UPGRADE_CANNON);
            structures["CannonUpgraded"] = true;
            EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD_IA, new object[] { _gold });
        }
        else {
            throw new Exception("No cumplen las condiciones para mejorar");
        }

        StartCoroutine(Wait(waitTime));
    }


    void Awake() {
        _ent = GetComponent<Entity>();

        var any = new State<IAAction>("any");
        var idle = new State<IAAction>("idle");
        var planStep = new State<IAAction>("planStep");
        var failStep = new State<IAAction>("failStep");
        var pickup = new State<IAAction>("pickup");
        var create = new State<IAAction>("create");
        var upgrade = new State<IAAction>("upgrade");
        var attack = new State<IAAction>("attack");
        var super_attack = new State<IAAction>("super_attack");
        var success = new State<IAAction>("success");


        failStep.OnEnter += a => { _ent.Stop(); Debug.Log("Plan failed"); };

        pickup.OnEnter += a => {
            Debug.Log("pickup.OnEnter");
            _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformPickUp;
            Debug.Log("pickup.OnEnter finish");
        };
        pickup.OnExit += a =>
        {

            Debug.Log("pickup.OnExit");
            action_started = false;
            _ent.OnHitItem -= PerformPickUp;
            Debug.Log("pickup.OnExit finish");
        };


        create.OnEnter += a => {
            print("entro en el create.onEnter");
            _ent.GoTo(_target.transform.position);
            _ent.OnHitItem += PerformCreate;
            print("salgo en el create.onEnter");
        };
        create.OnExit += a =>{
            action_started = false;
            _ent.OnHitItem -= PerformCreate;
        };


        attack.OnEnter += a => {
            _ent.GoTo(_target.transform.position);
            _ent.OnHitItem += PerformAttack;
        };
        attack.OnExit += a =>
        {
            action_started = false;
            _ent.OnHitItem -= PerformAttack;
        };

       super_attack.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformSuperAttack; };
        super_attack.OnExit += a =>
        {
            action_started = false;
            _ent.OnHitItem -= PerformSuperAttack;
        };



        upgrade.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformUpgrade; };
        upgrade.OnExit += a =>  {
            action_started = false;
            _ent.OnHitItem -= PerformUpgrade;
        };

        planStep.OnEnter += a => {
            if (shouldRePlan) {

                var planner = this.GetComponent<Planner>();
                _plan= planner.RecalculatePlan(GetCurState());
                shouldRePlan = false;
            }
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
                _fsm.Feed(IAAction.Success);
			}
		};


        success.OnEnter += a => { Debug.Log("Success"); };
		success.OnUpdate += () => { _ent.Jump(); };
		
		StateConfigurer.Create(any)
			.SetTransition(IAAction.NextStep, planStep)
			.SetTransition(IAAction.FailedStep, idle)
			.Done();

        StateConfigurer.Create(planStep)
            .SetTransition(IAAction.PickUp, pickup)
            .SetTransition(IAAction.Create, create)
            .SetTransition(IAAction.Attack, attack)
            .SetTransition(IAAction.SuperAttack, super_attack)
            .SetTransition(IAAction.Upgrade, upgrade)
            .SetTransition(IAAction.Success, success)
			.Done();
        
		_fsm = new EventFSM<IAAction>(idle, any);
    }

    private GOAPState GetCurState()
    {
        WorldSpace world = new WorldSpace();

        world.gold = _gold;
        world.hasDefense = structures["Defense"];
        world.cannon = structures["Cannon"] ? "cannon" : "";
        if (structures["CannonUpgraded"])
        {
            world.cannon = "cannon upgraded";
        }
        world.defenseIsRepaired = structures["DefenseUpgraded"];
        world.enemyLife = playerCore.GetLife();
        world.bullet = bullets;
        GOAPState cur = new GOAPState(world);
        return cur;
    }

    internal void NextStep()
    {
        _fsm.Feed(IAAction.NextStep);
    }

    public void SetPlan(List<Tuple<IAAction, Item>> plan) {
		_plan = plan;
		//_fsm.Feed(TipitoAction.NextStep);
	}

    void Update ()
    {
        _fsm.Update();
	}
}
