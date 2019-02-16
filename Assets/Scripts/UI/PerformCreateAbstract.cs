using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerformCreateAbstract : MonoBehaviour
{
    protected void TriggerCreate(string constant, ItemType type)
    {
        EventsManager.TriggerEvent(constant, new object[] { type });
    }
}
