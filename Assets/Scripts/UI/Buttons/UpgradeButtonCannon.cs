using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButtonCannon : PerformSomethingButtonAbstract
{
    public void PerformUpgrade()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_UPGRADE);
    }
}
