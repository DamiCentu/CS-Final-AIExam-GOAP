using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerformSomethingButtonAbstract : MonoBehaviour
{
    public Item itemToCall;

    protected void ButtonTrigger(string constant)
    {
        EventsManager.TriggerEvent(constant, new object[] { itemToCall });
    }
}
