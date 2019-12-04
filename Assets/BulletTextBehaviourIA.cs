using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTextBehaviourIA : UpdateableTextAbstract
{
    void Start()
    {
        EventsManager.SubscribeToEvent(EventsConstants.UI_UPDATE_BULLETS_IA, OnUpdateBulletUI);
        SetText("Bullets: 0");
    }

    private void OnUpdateBulletUI(object[] parameterContainer)
    {
        SetText("Bullets: " + ((int)parameterContainer[0]).ToString());
    }
}
