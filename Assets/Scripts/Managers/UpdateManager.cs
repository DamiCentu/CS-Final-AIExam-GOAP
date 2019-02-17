using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    event Action _updates = delegate { };

    void Awake()
    {
        EventsManager.SubscribeToEvent(EventsConstants.SUBSCRIBE_UPDATE, onSubscribeToUpdate);
        EventsManager.SubscribeToEvent(EventsConstants.DESUBSCRIBE_UPDATE, onDesubscribeToUpdate);
    }

    private void onSubscribeToUpdate(object[] param)
    {
        if(param[0] != null && param[0] is Action)
        {
            _updates += (Action)param[0];
        }
        else
        {
            throw new Exception("amigo plis, estas pasando mal el metodo en subscribe");
        }
    }

    private void onDesubscribeToUpdate(object[] param)
    {
        if (param[0] != null && param[0] is Action)
        {
            _updates -= (Action)param[0]; 
        }
        else
        {
            throw new Exception("amigo plis, estas pasando mal el metodo en desubscribe");
        }
    }

    void Update()
    {
        _updates();
    }
}
