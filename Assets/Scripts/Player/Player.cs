using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    Entity _ent;

    void Awake()
    {
        _ent = GetComponent<Entity>();
        if (!_ent)
            throw new Exception("Entity component null");
    }

    void Start ()
    {
        EventsManager.TriggerEvent(EventsConstants.SUBSCRIBE_UPDATE, (Action)onUpdate);
	}
	
	void onUpdate ()
    {

	}

    void OnDisable()
    {
        EventsManager.TriggerEvent(EventsConstants.DESUBSCRIBE_UPDATE, (Action)onUpdate);
    }

    void OnDestroy()
    {
        EventsManager.TriggerEvent(EventsConstants.DESUBSCRIBE_UPDATE, (Action)onUpdate);
    }
}
