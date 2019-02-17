using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButtonDefense : PerformCreateAbstract
{
    public void PerformCreate()
    {
        TriggerCreate(EventsConstants.PLAYER_REQUEST_CREATE, ItemType.Defense);
    }
}
