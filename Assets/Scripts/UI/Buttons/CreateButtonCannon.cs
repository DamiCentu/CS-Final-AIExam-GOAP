using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButtonCannon : PerformSomethingButtonAbstract
{
    public void PerformCreate()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_CREATE);
    }
}
