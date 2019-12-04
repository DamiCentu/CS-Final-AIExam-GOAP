using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldTextBehaviourIA : UpdateableTextAbstract
{
    void Start()
    {
        EventsManager.SubscribeToEvent(EventsConstants.UI_UPDATE_GOLD_IA, OnUpdateGoldUI);
        SetText("Gold: 0");
    }

    private void OnUpdateGoldUI(object[] parameterContainer)
    {
        SetText("Gold: " + ((int)parameterContainer[0]).ToString());
    }
}
