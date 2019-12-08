using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButtonDefense : PerformSomethingButtonAbstract
{
    public void PerformUpgrade()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_UPGRADE);
    }

    private void Start()
    {
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_CREATE, onPlayerCreate);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_DEFENSE_DESTROYED, onPlayerDefenseDestroyed);
        gameObject.SetActive(false);
    }

    private void onPlayerDefenseDestroyed(object[] parameterContainer)
    {
        gameObject.SetActive(false);
    }

    private void onPlayerCreate(object[] parameterContainer)
    {
        var type = (ItemType)parameterContainer[0];

        if (type == ItemType.Defense)
            gameObject.SetActive(true);
    }
}
