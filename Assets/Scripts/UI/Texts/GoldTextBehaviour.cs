using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldTextBehaviour : UpdateableTextAbstract
{
    void Start()
    {
        EventsManager.SubscribeToEvent(EventsConstants.UI_UPDATE_GOLD, OnUpdateGoldUI);
        SetText( "Gold: 0");
    }

    private void OnUpdateGoldUI(object[] parameterContainer)
    {
        SetText("Gold: " + ((int)parameterContainer[0]).ToString());
    }
}
