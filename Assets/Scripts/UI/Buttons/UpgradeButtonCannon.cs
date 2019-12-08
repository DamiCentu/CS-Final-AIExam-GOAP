using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonCannon : PerformSomethingButtonAbstract
{
    private void Start()
    {
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_CREATE, onPlayerCreate);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_UPGRADE, onPlayerUpgrade);
        gameObject.SetActive(false);
    }

    private void onPlayerUpgrade(object[] parameterContainer)
    {
        var type = (ItemType)parameterContainer[0];

        if (type == ItemType.Cannon)
            gameObject.SetActive(false);
    }

    private void onPlayerCreate(object[] parameterContainer)
    {
        var type = (ItemType)parameterContainer[0];

        if (type == ItemType.Cannon)
            gameObject.SetActive(true);
    }

    public void PerformUpgrade()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_UPGRADE);
    }
}
