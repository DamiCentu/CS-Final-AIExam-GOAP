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
    Entity _ent;

    int _gold = 0;
    int _bullets = 0;

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
        //EventsManager.TriggerEvent(EventsConstants.SUBSCRIBE_UPDATE, (Action)onUpdate);

        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_REQUEST_CREATE, OnRequestCreate);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_REQUEST_UPGRADE, OnRequestUpgrade);
        EventsManager.SubscribeToEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, OnBlockOrUnblockPlayer);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_REQUEST_MINING, OnRequestMining);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_REQUEST_SHOOT, OnRequestShoot);
    }

    private void OnRequestShoot(object[] parameterContainer)
    {
        if(_bullets > 0)
        {
            var item = (Item)parameterContainer[0];
            SimpleRequest(item, PerformAttack);
        }
        else
        {
            EventsManager.TriggerEvent(EventsConstants.PLAYER_NOT_ENOUGH_BULLETS_NOTIFICATION);
        }
    }

    private void OnRequestMining(object[] parameterContainer)
    {
        var item = (Item)parameterContainer[0];
        SimpleRequest(item, PerformPickUp);
    }

    void SimpleRequest(Item item, Action<Entity, Item> action)
    {
        if (!_canDoAnythingElse)
        {
            EventsManager.TriggerEvent(EventsConstants.PLAYER_OCCUPIED_NOTIFICATION);
            return;
        }
        
        EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { false });
        _ent.GoTo(item.transform.position, item);
        _ent.OnReachDestinationWithItem += action;
    }

    protected override void PerformPickUp(Entity ent, Item item)
    {
        EventsManager.TriggerEvent(EventsConstants.PLAYER_MINING);
        StartCoroutine(Wait(timeToPickup,
            () =>
            {
                if (item)
                    _gold += item.miningGoldValueGiven;

                EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD, new object[] { _gold });
                EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { true });
                _ent.OnReachDestinationWithItem -= PerformPickUp;
            }));
    }

    void RequestWithGold(Item item, int goldCost, Action<Entity, Item> action)
    {
        if (!_canDoAnythingElse)
        {
            EventsManager.TriggerEvent(EventsConstants.PLAYER_OCCUPIED_NOTIFICATION);
            return;
        }

        if (_gold >= goldCost)
        {
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { false });
            _ent.GoTo(item.transform.position, item);
            _ent.OnReachDestinationWithItem += action;
        }
        else
        {
            EventsManager.TriggerEvent(EventsConstants.PLAYER_NOT_ENOUGH_GOLD_NOTIFICATION);
        }
    }

    private void OnRequestUpgrade(object[] parameterContainer)
    {
        var item = (Item)parameterContainer[0];
        RequestWithGold(item, item.itemCostToUpgrade, PerformUpgrade);
    }

    protected override void PerformUpgrade(Entity ent, Item item)
    {
        if (item.type == ItemType.Defense)
        {
            var defense = item.GetComponent<Defense>();
            defense.Upgrade();
        }

        else if (item.type == ItemType.Cannon)
        {
            var cannon = item.GetComponent<Cannon>();
            cannon.Upgrade();
        }

        else if (item.type == ItemType.WorkTable)
        {
            var workTable = item.GetComponent<WorkTable>();
            workTable.UpgradeBullet();
        }

        _gold -= item.itemCostToUpgrade;
        _ent.OnReachDestinationWithItem -= PerformUpgrade;
        EventsManager.TriggerEvent(EventsConstants.PLAYER_UPGRADE, new object[] { item.type });
        EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD, new object[] { _gold });
        EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { true });
    }

    private void OnBlockOrUnblockPlayer(object[] parameterContainer)
    {
        _canDoAnythingElse = (bool)parameterContainer[0];
    }

    private void OnRequestCreate(object[] parameterContainer)
    {
        var item = (Item)parameterContainer[0];
        RequestWithGold(item, item.itemCostToCreate, PerformCreate);
    }

    protected override void PerformCreate(Entity ent, Item item)
    {
        if (item.type == ItemType.Defense)
        {
            var defense = item.GetComponent<Defense>();
            defense.Activate().Set();
        }

        if (item.type == ItemType.Cannon)
        {
            var cannon = item.GetComponent<Cannon>();
            cannon.Activate();
        }


        if (item.type == ItemType.WorkTable)
        {
            var workTable = item.GetComponent<WorkTable>();
            workTable.CreateBullet();
            _bullets++;
            EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_BULLETS, new object[] { _bullets });
        }

        _gold -= item.itemCostToCreate;
        _ent.OnReachDestinationWithItem -= PerformCreate;
        EventsManager.TriggerEvent(EventsConstants.PLAYER_CREATE, new object[] { item.type });
        EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_GOLD, new object[] { _gold });
        EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { true });
    }

    protected override void PerformOpen(Entity ent, Item item)
    {
        var key = ent.items.FirstOrDefault(it => it.type == ItemType.Key);
        var door = item.GetComponent<Door>();
        if (door && key)
        {
            door.Open();
            //Consume key
            Destroy(ent.Removeitem(key).gameObject);
        }
        else
        {

        }
    }

    protected override void PerformAttack(Entity us, Item item)
    {
        if (item.type == ItemType.Cannon)
        {
            var cannon = item.GetComponent<Cannon>();
            cannon.AttackNormal();
            _bullets--;
            EventsManager.TriggerEvent(EventsConstants.UI_UPDATE_BULLETS, new object[] { _bullets });
        }
        EventsManager.TriggerEvent(EventsConstants.PLAYER_ATTACK, new object[] { item.type });
        _ent.OnReachDestinationWithItem -= PerformAttack;
    }

    protected override void PerformWait(Entity ent, Item item)
    {
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
}
