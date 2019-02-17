using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour {

    public GameObject defensePrefab;
    public GameObject cannonPrefab;

    public float maxRaycastDistance = 300;
    public LayerMask hoverLayer;

    GameObject _goToInstantiate;
    IEnumerable _allNodes;
    Navigation _nav;
    Camera _cam;

    ItemType _lastTipe;

    void Start () {
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_CREATE, OnCreate);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_UPGRADE, OnUpgrade);

        _nav = Navigation.instance;
        if (!_nav)
            throw new Exception("navigation null");

        _cam = Camera.main;

        if (!_cam)
            throw new Exception("Cam null");
    }

    private void OnUpgrade(object[] parameterContainer)
    {
        throw new NotImplementedException();
    }

    private void OnCreate(object[] parameterContainer)
    {
        EventsManager.TriggerEvent(EventsConstants.SUBSCRIBE_UPDATE, new object[] { (Action)OnUpdate });

        if ((ItemType)parameterContainer[0] == ItemType.Defense)
        {
            _goToInstantiate = Instantiate(defensePrefab);
            _lastTipe = ItemType.Defense;
        }
        else if ((ItemType)parameterContainer[0] == ItemType.Cannon)
        {
            _goToInstantiate = Instantiate(cannonPrefab);
            _lastTipe = ItemType.Cannon;
        }
        else
        {
            EventsManager.TriggerEvent(EventsConstants.DESUBSCRIBE_UPDATE, new object[] { (Action)OnUpdate });
        }
    }

    void OnUpdate()
    {
        RaycastHit hit;
        var ray = _cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * maxRaycastDistance, Color.yellow, 1f);

        if (Physics.Raycast(ray, out hit, maxRaycastDistance, hoverLayer))
        {
            _goToInstantiate.transform.position = _nav.NearestTo(hit.collider.transform.position).position;
        }

        if(Input.GetMouseButtonDown(0))
        {
            EventsManager.TriggerEvent(EventsConstants.DESUBSCRIBE_UPDATE, new object[] { (Action)OnUpdate });
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { true });
            EventsManager.TriggerEvent(EventsConstants.ITEM_CREATED, new object[] { _lastTipe });

        }
    }
}
