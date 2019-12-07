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
    Wait,
    SuperAttack
}

public class Tipito : PlayerAndIABaseBehaviour
{
    EventFSM<TipitoAction> _fsm;
	Item _target;
    bool action_started = false;
	Entity _ent;
    public bool shouldRePlan=false;
    public float waitTime=2f;
    private bool planAgresive = true;

    IEnumerable<Tuple<TipitoAction, Item>> _plan;
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
            shouldRePlan = true;
            Planner p = GetComponent<Planner>();
            p.SetAggressivePlan(true);
        }
    }

    private void StartDefensivePlan()
    {
        if (planAgresive)
        {
            planAgresive = false;
            shouldRePlan = true;
            Planner p = GetComponent<Planner>();
            p.SetAggressivePlan(false);
        }
    }

    private void IALifeChange(object[] parameterContainer)
    {
        int life = (int)parameterContainer[0];
        if (life <= 75)
        {
            StartDefensivePlan();
        }
        else
        {
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
            EventsManager.TriggerEvent(EventsConstants.IA_CREATE_DEFENSE);
            var defense = item.GetComponent<Defense>();
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
       /* if (count >1000) //TODO QUITAR Y VER EL REPLAN
        {
            count = 0;
            shouldRePlan = true;
        }*/
        _fsm.Feed(TipitoAction.NextStep);
       // count++;

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
    /*    if (item.type == ItemType.WorkTable)
        {
            var workTable = item.GetComponent<WorkTable>();
            workTable.UpgradeBullet();
            EventsManager.TriggerEvent(EventsConstants.IA_UPGRADE_BULLET);
        }*/
        StartCoroutine(Wait(waitTime));
    }

    void NextStep(Entity ent, MapNode wp, bool reached) {
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
        var super_attack = new State<TipitoAction>("super_attack");
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
            action_started = false;
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
        create.OnExit += a =>{
            _ent.OnHitItem -= PerformCreate;
            action_started = false;
        };


        attack.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformAttack; };
        attack.OnExit += a =>
        {
            _ent.OnHitItem -= PerformAttack;
            action_started = false;
        };

       super_attack.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformSuperAttack; };
        super_attack.OnExit += a =>
        {
            _ent.OnHitItem -= PerformSuperAttack;
            action_started = false;
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
                print("Cheeee cambie el plan eh");
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
            .SetTransition(TipitoAction.SuperAttack, super_attack)
            .SetTransition(TipitoAction.Upgrade, upgrade)
            .SetTransition(TipitoAction.Open, open)
            .SetTransition(TipitoAction.Success, success)
            .SetTransition(TipitoAction.Wait, wait)
			.Done();
        
		_fsm = new EventFSM<TipitoAction>(idle, any);
    }

    private GOAPState GetCurState()
    {
        GOAPState cur = new GOAPState();
        cur.intValues["hasGold"] = _gold;
        cur.boolValues["hasDefense"] = structures["Defense"];
        cur.boolValues["hasCannon"] = structures["Cannon"];
        cur.boolValues["UpgradeCannon"] = structures["CannonUpgraded"];
        cur.boolValues["UpgradeDefense"] = structures["DefenseUpgraded"];
        cur.floatValues["EnemyLife"] = playerCore.GetLife();
    cur.strignValues["bullet"] = bullets>0? "Normal Bullet":"";
        return cur;
    }

    internal void NextStep()
    {
        _fsm.Feed(TipitoAction.NextStep);
    }

    public void SetPlan(List<Tuple<TipitoAction, Item>> plan) {
		_plan = plan;
		//_fsm.Feed(TipitoAction.NextStep);
	}

    void Update ()
    {
        _fsm.Update();
	}
}
