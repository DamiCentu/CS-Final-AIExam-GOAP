using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButtonBullet : PerformSomethingButtonAbstract
{
    public void PerformUpgrade()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_UPGRADE);
    }
}
