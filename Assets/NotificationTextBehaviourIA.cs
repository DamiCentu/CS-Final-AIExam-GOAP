using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationTextBehaviourIA : UpdateableTextAbstract
{
    public float timeShowingNotification = 1.5f;

    Queue<string> textsQueue = new Queue<string>();
    WaitForSeconds _waitShowingNotification;
    bool _routineRunning = false;



    void Start()
    {
        _waitShowingNotification = new WaitForSeconds(timeShowingNotification);
        EventsManager.SubscribeToEvent(EventsConstants.IA_MINNING, OnIA_MINNING);
        EventsManager.SubscribeToEvent(EventsConstants.IA_SHOOTING, OnIA_SHOOTING);
        EventsManager.SubscribeToEvent(EventsConstants.IA_CREATE_BULLET, OnIA_CREATE_BULLET);
        EventsManager.SubscribeToEvent(EventsConstants.IA_CREATE_CANNON, OnIA_CREATE_CANNON);
        EventsManager.SubscribeToEvent(EventsConstants.IA_CREATE_DEFENSE, OnIA_CREATE_DEFENS);
        EventsManager.SubscribeToEvent(EventsConstants.IA_UPGRADE_BULLET, OnIA_UPGRADE_BULLET);
        EventsManager.SubscribeToEvent(EventsConstants.IA_UPGRADE_CANNON, OnIA_UPGRADE_CANNON);
        EventsManager.SubscribeToEvent(EventsConstants.IA_UPGRADE_DEFENSE, OnIA_UPGRADE_DEFENSE);
        SetText("");
    }

    private void OnIA_MINNING(object[] parameterContainer)
    {
        HandleText("Minning...");
    }

    private void OnIA_UPGRADE_DEFENSE(object[] parameterContainer)
    {
        HandleText("Upgrading defense...");
    }

    private void OnIA_UPGRADE_CANNON(object[] parameterContainer)
    {
        HandleText("Upgrading cannon...");
    }

    private void OnIA_UPGRADE_BULLET(object[] parameterContainer)
    {
        HandleText("Upgrading bullet...");
    }

    private void OnIA_CREATE_DEFENS(object[] parameterContainer)
    {
        HandleText("Creating defenses...");
    }

    private void OnIA_CREATE_CANNON(object[] parameterContainer)
    {
        HandleText("Creating cannon...");
    }

    private void OnIA_CREATE_BULLET(object[] parameterContainer)
    {
        HandleText("Creating bullet...");
    }

    private void OnIA_SHOOTING(object[] parameterContainer)
    {
        HandleText("Shooting...");
    }

    void HandleText(string text)
    {
        textsQueue.Enqueue(text);
        if (!_routineRunning)
        {
            StartCoroutine(TextNotificationRoutine());
        }
    }

    IEnumerator TextNotificationRoutine()
    {
        _routineRunning = true;
        while (textsQueue.Any())
        {
            SetText(textsQueue.Dequeue());
            yield return _waitShowingNotification;
            SetText("");
        }
        _routineRunning = false;
    }
}
