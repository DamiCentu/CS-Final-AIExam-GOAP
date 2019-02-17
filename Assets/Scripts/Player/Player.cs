using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using IA2;

public class Player : PlayerAndIABaseBehaviour
{
    public LayerMask clickLayerHit;
    public float maxRaycastClickDistance = 500f;
    public float timeToPickup = 1f;
    public int goldToCreateDefense = 30;
    public int goldToCreateCannon = 60;
    Entity _ent;
    Item _target;

    int _gold = 0;

    bool _canDoAnythingElse = true;

    Camera _cam;

    void Awake()
    {
        _ent = GetComponent<Entity>();
        if (!_ent)
            throw new Exception("Entity component null");

        _cam = Camera.main;

        if (!_cam)
            throw new Exception("Cam null");
    }

    void Start ()
    {
        EventsManager.TriggerEvent(EventsConstants.SUBSCRIBE_UPDATE, (Action)onUpdate);

        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_REQUEST_CREATE, OnCreateDefense);
        EventsManager.SubscribeToEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, OnBlockOrUnblockPlayer);
    }

    private void OnBlockOrUnblockPlayer(object[] parameterContainer)
    {
        _canDoAnythingElse = (bool)parameterContainer[0];
    }

    private void OnCreateDefense(object[] parameterContainer)
    {
        if (!_canDoAnythingElse)
        {
            EventsManager.TriggerEvent(EventsConstants.PLAYER_OCUPED);
            return;
        }

        if ((ItemType)parameterContainer[0] == ItemType.Defense && _gold >= goldToCreateDefense)
        {
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { false });
            _gold -= goldToCreateDefense;
            EventsManager.TriggerEvent(EventsConstants.PLAYER_CREATE, new object[] { (ItemType)parameterContainer[0] });
            EventsManager.TriggerEvent(EventsConstants.UPDATE_GOLD_UI, new object[] { _gold });
        }
        else if ((ItemType)parameterContainer[0] == ItemType.Cannon && _gold >= goldToCreateCannon)
        {
            _gold -= goldToCreateCannon;
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { false });
            EventsManager.TriggerEvent(EventsConstants.PLAYER_CREATE, new object[] { (ItemType)parameterContainer[0] });
            EventsManager.TriggerEvent(EventsConstants.UPDATE_GOLD_UI, new object[] { _gold });
        }            
        else
        {
            EventsManager.TriggerEvent(EventsConstants.PLAYER_NOT_ENOUGH_GOLD);
        }
    }

    void onUpdate()
    {
        //   _fsm.Update();
        if (_canDoAnythingElse && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = _cam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * maxRaycastClickDistance, Color.yellow, 1f);

            if (Physics.Raycast(ray, out hit, maxRaycastClickDistance, clickLayerHit))
            {
                var item = hit.collider.GetComponent<Item>();
                if (item)
                {
                    _target = item;
                    CalculateActionByItem(item);
                }
            }
        }       
    }

    void CalculateActionByItem(Item item)
    {
        if(item.type == ItemType.Mine)
        {
            _ent.GoTo(item.transform.position , item);
            _ent.OnReachDestinationWithItem += PerformPickUp;
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { false });
        }
    }

    void OnDisable()
    {
        EventsManager.TriggerEvent(EventsConstants.DESUBSCRIBE_UPDATE, (Action)onUpdate);
    }

    void OnDestroy()
    {
        EventsManager.TriggerEvent(EventsConstants.DESUBSCRIBE_UPDATE, (Action)onUpdate);
    }

    protected override void PerformOpen(Entity ent, Item item)
    {
        if (item != _target) return;
        Debug.Log("Player open");

        var key = ent.items.FirstOrDefault(it => it.type == ItemType.Key);
        var door = item.GetComponent<Door>();
        if (door && key)
        {
            door.Open();
            //Consume key
            Destroy(ent.Removeitem(key).gameObject);
            //_fsm.Feed("");
        }
        else
        {
            //_fsm.Feed("");
        }
    }

    protected override void PerformPickUp(Entity ent, Item item)
    {
        Debug.Log("Player pickup");
        StartCoroutine(Wait(timeToPickup, 
            () => 
        {
            if(item)
                _gold += item.goldValue;

            EventsManager.TriggerEvent(EventsConstants.UPDATE_GOLD_UI, new object[] { _gold });
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { true });
            _ent.OnReachDestinationWithItem -= PerformPickUp;
        }));
    }

    protected override void PerformCreate(Entity ent, Item item)
    {
        if (item != _target) return;
        Debug.Log("Player create");
        //	_ent.AddItem(other);
        if (item.type == ItemType.Defense)
        {
            var defense = item.GetComponent<Defense>();
            defense.Activate();
        }

        if (item.type == ItemType.Cannon)
        {
            var cannon = item.GetComponent<Cannon>();
            cannon.Create();
        }

        //_fsm.Feed("");
    }

    protected override void PerformAttack(Entity us, Item item)
    {
        if (item != _target) return;
        Debug.Log("Player attack");

        if (item.type == ItemType.Cannon)
        {
            var cannon = item.GetComponent<Cannon>();
            cannon.Attack();
        }
        //_fsm.Feed("");
    }

    protected override void PerformWait(Entity ent, Item item)
    {
        if (item != _target)
            return;
        StartCoroutine(Wait(1.0f));
    }

    IEnumerator Wait(float waitTime, Action action = null)
    {
        print("Player waitForSeconds!");
        yield return new WaitForSeconds(waitTime);
        if(action != null)
            action();
        print("Player ya espere!");
    }

    protected override void PerformUpgrade(Entity ent, Item item)
    {
        if (item != _target) return;
        Debug.Log("Player upgrade");
        if (item.type == ItemType.Defense)
        {
            var defense = item.GetComponent<Defense>();
            defense.Upgrade();
        }

        if (item.type == ItemType.Cannon)
        {
            var cannon = item.GetComponent<Cannon>();
            cannon.Upgrade();
        }
        //_fsm.Feed("");
    }
}
