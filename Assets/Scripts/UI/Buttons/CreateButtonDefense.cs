using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateButtonDefense : PerformSomethingButtonAbstract
{
    public void PerformCreate()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_CREATE);
    }
}
