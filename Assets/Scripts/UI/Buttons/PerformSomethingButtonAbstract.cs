using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerformSomethingButtonAbstract : MonoBehaviour
{
    public Item itemToCall;

    protected void ButtonTrigger(string constant , Item _itemToCall = null)
    {
        if(!_itemToCall)
            EventsManager.TriggerEvent(constant, new object[] { itemToCall });
        else
            EventsManager.TriggerEvent(constant, new object[] { _itemToCall });
    }
}
