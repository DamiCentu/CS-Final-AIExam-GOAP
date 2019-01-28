using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GOAPState
{
    public Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
    public Dictionary<string, int> intValues = new Dictionary<string, int>();
    public GOAPAction generatingAction = null;
    public int step = 0;

    #region CONSTRUCTOR
    public GOAPState(GOAPAction gen = null)
    {
        generatingAction = gen;
    }

    public GOAPState(GOAPState source, GOAPAction gen = null)
    {
        SetValues(source.boolValues, boolValues);
        SetValues(source.intValues, intValues);
        generatingAction = gen;
    }

    private void SetValues<T>(Dictionary<string, T> source, Dictionary<string, T> list)
    {
        foreach (var elem in source)
        {
            if (list.ContainsKey(elem.Key))
                list[elem.Key] = elem.Value;
            else
                list.Add(elem.Key, elem.Value);
        }
    }
    #endregion

    public override bool Equals(object obj)
    {
        var other = obj as GOAPState;
        var result =
            other != null
            && other.generatingAction == generatingAction       //Very important to keep! TODO: REVIEW
            && other.boolValues.Count == boolValues.Count
            && other.boolValues.All(kv => kv.In(boolValues))
            && other.intValues.Count == intValues.Count
            && other.intValues.All(kv => kv.In(intValues));
        //&& other.values.All(kv => values.Contains(kv));
        return result;
    }

    public override int GetHashCode()
    {
        //Better hashing but slow.
        //var x = 31;
        //var hashCode = 0;
        //foreach(var kv in values) {
        //	hashCode += x*(kv.Key + ":" + kv.Value).GetHashCode);
        //	x*=31;
        //}
        //return hashCode;

        //Heuristic count+first value hash multiplied by polynomial primes
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
    }
}
