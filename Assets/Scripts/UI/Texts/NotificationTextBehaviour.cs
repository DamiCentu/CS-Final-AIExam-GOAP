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
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_NOTIFICATION, OnPlayerNotification);

        SetText("");
    }

    private void OnPlayerNotification(object[] parameterContainer)
    {
        HandleText(parameterContainer[0] as string);
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
