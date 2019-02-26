using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineGoldButton : PerformSomethingButtonAbstract
{
    public void PerformMine()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_MINING);
    }
}
