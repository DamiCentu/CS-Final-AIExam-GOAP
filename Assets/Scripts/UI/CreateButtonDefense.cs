using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateButtonDefense : PerformCreateAbstract
{
    Text _text;

    private void Awake()
    {
        EventsManager.SubscribeToEvent(EventsConstants.ITEM_CREATED, OnItemCreated);
        _text = GetComponentInChildren<Text>();
        if(!_text)
            throw new Exception("Text null");
    }

    private void OnItemCreated(object[] parameterContainer)
    {
        if((ItemType)parameterContainer[0] == ItemType.Defense)
        {
            _text.text = "UPGRADE DEFENSE";
        }
    }

    public void PerformCreate()
    {
        TriggerCreate(EventsConstants.PLAYER_REQUEST_CREATE, ItemType.Defense);
    }
}
