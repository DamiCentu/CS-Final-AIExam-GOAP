using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class NotificationTextBehaviour : UpdateableTextAbstract
{
    public float timeShowingNotification = 1.5f;

    Queue<string> textsQueue = new Queue<string>();
    WaitForSeconds _waitShowingNotification;
    bool _routineRunning = false;

    void Start()
    {
        _waitShowingNotification = new WaitForSeconds(timeShowingNotification);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_NOT_ENOUGH_BULLETS_NOTIFICATION, OnPlayerNotEnoughBullets);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_OCCUPIED_NOTIFICATION, OnPlayerOccupied);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_NOT_ENOUGH_GOLD_NOTIFICATION, OnPlayerNotEnoughGold);

        SetText("");
    }

    private void OnPlayerNotEnoughGold(object[] parameterContainer)
    {
        HandleText("Player not enough gold");
    }

    private void OnPlayerOccupied(object[] parameterContainer)
    {
        HandleText("Player occupied");
    }

    void OnPlayerNotEnoughBullets(object[] parameterContainer)
    {
        HandleText("Player not enough bullets");
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
