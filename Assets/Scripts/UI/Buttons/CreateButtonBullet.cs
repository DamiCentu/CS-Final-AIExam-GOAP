using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButtonBullet : PerformSomethingButtonAbstract
{
    public void PerformCreate()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_CREATE);
    }
}
