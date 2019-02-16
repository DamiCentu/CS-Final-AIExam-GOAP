using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour {

	void Start () {
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_CREATE, OnCreate);        
    }

    private void OnCreate(object[] parameterContainer)
    {
        if((ItemType)parameterContainer[0] == ItemType.Defense)
        {

        }
        else if ((ItemType)parameterContainer[0] == ItemType.Cannon)
        {

        }
    }
}
