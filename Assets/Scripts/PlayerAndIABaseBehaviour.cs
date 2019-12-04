using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAndIABaseBehaviour : MonoBehaviour
{
    protected virtual void PerformOpen(Entity ent, Item item) { }
    protected virtual void PerformPickUp(Entity ent, Item item) { }
    protected virtual void PerformCreate(Entity ent, Item item) { }
    protected virtual void PerformAttack(Entity ent, Item item) { }
    protected virtual void PerformSuperAttack(Entity ent, Item item) { }
    protected virtual void PerformWait(Entity ent, Item item) { }
    protected virtual void PerformUpgrade(Entity ent, Item item) { }
}
