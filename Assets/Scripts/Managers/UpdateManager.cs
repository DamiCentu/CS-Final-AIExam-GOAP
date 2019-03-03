using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    event Action _updates = delegate { };
    event Action _fixedUpdates = delegate { };

    void Awake()
    {
        EventsManager.SubscribeToEvent(EventsConstants.SUBSCRIBE_UPDATE, OnSubscribeToUpdate);
        EventsManager.SubscribeToEvent(EventsConstants.DESUBSCRIBE_UPDATE, OnDesubscribeToUpdate);

        EventsManager.SubscribeToEvent(EventsConstants.SUBSCRIBE_FIXED_UPDATE, OnSubscribeToFixedUpdate);
        EventsManager.SubscribeToEvent(EventsConstants.DESUBSCRIBE_FIXED_UPDATE, OnDesubscribeToFixedUpdate);

        EventsManager.SubscribeToEvent(EventsConstants.CLEAR_ALL_UPDATES, OnClearAllUpdates);
    }

    void OnSubscribeToUpdate(object[] param)
    {
        if(param[0] != null && param[0] is Action)
        {
            _updates += (Action)param[0];
        }
        else
        {
            throw new Exception("amigo plis, estas pasando mal el metodo en subscribe update");
        }
    }

    void OnDesubscribeToUpdate(object[] param)
    {
        if (param[0] != null && param[0] is Action)
        {
            _updates -= (Action)param[0]; 
        }
        else
        {
            throw new Exception("amigo plis, estas pasando mal el metodo en desubscribe update");
        }
    }

    void OnSubscribeToFixedUpdate(object[] param)
    {
        if (param[0] != null && param[0] is Action)
        {
            _fixedUpdates += (Action)param[0];
        }
        else
        {
            throw new Exception("amigo plis, estas pasando mal el metodo en subscribe fixed update");
        }
    }

    void OnDesubscribeToFixedUpdate(object[] param)
    {
        if (param[0] != null && param[0] is Action)
        {
            _fixedUpdates -= (Action)param[0];
        }
        else
        {
            throw new Exception("amigo plis, estas pasando mal el metodo en desubscribe fixed update");
        }
    }

    void OnClearAllUpdates(object[] param)
    {
        _updates = delegate { };
        _fixedUpdates = delegate { };
    }

    void FixedUpdate()
    {
        _fixedUpdates();
    }

    void Update()
    {
        _updates();
    }
}
