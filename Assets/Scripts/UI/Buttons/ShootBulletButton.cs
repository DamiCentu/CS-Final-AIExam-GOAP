using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBulletButton : PerformSomethingButtonAbstract
{
    public void PerformShoot()
    {
        ButtonTrigger(EventsConstants.PLAYER_REQUEST_SHOOT);
    }
}
