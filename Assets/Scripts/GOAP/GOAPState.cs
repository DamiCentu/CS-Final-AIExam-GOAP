using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GOAPState
{
    public WorldSpace worldSpace;
    public GOAPAction generatingAction = null;
    public int step = 0;

    #region CONSTRUCTOR
    public GOAPState(GOAPAction gen = null)
    {
        generatingAction = gen;
    }
    public GOAPState(WorldSpace world, GOAPAction gen = null)
    {
        worldSpace = world;
        generatingAction = gen;
    }

    public GOAPState(GOAPState source, GOAPAction gen = null)
    {
        this.worldSpace = source.worldSpace;
        generatingAction = gen;
    }


    #endregion

    public override bool Equals(object obj)
    {
        var other = obj as GOAPState;
        var result =
            other != null
            && other.generatingAction == generatingAction       //Very important to keep! TODO: REVIEW
            && other.worldSpace == this.worldSpace;
        //&& other.values.All(kv => values.Contains(kv));
        return result;
    }

    internal bool PassiveComparison(WorldSpace other)
    {
        return worldSpace.PassiveComparison(other);
    }

    internal bool AgressiveComparison(WorldSpace other)
    {
        return worldSpace.AgressiveComparison(other);
    }

    internal bool ReachedGoal(WorldSpace other)
    {
        return worldSpace.ReachGoal( other);
    }

    internal float PassiveHeuristic(WorldSpace other)
    {
        return worldSpace.PassiveHeuristic(other);
    }

    internal float AgressiveHeuristic(WorldSpace other)
    {
        return worldSpace.AgressiveHeuristic(other);
    }
    /*
public override int GetHashCode()
{
   return boolValues.Count == 0 ? 0 : 31 * boolValues.Count + 31 * 31 * boolValues.First().GetHashCode();
}

public override string ToString()
{
   var str = "";
   foreach (var kv in boolValues.OrderBy(x => x.Key))
   {
       str += (string.Format("{0:12} : {1}\n", kv.Key, kv.Value));
   }
   return ("--->" + (generatingAction != null ? generatingAction.name : "NULL") + "\n" + str);
}*/
}
