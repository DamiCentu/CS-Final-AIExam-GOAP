using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButtonDefense : PerformSomethingButtonAbstract
{
    bool _oneItemAtLeastCreated;

	void Start ()
    {
        EventsManager.SubscribeToEvent(EventsConstants.ITEM_CREATED, OnItemCreated);
    }

    private void OnItemCreated(object[] parameterContainer)
    {
        _oneItemAtLeastCreated = true;
    }

    void PerformUpgrade()
    {
        if(_oneItemAtLeastCreated)
            TriggerCreate(EventsConstants.PLAYER_REQUEST_UPGRADE, ItemType.Defense);
    }
}
