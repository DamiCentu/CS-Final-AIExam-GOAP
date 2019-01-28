using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPAction
{
    public Dictionary<string, bool> preconditionsBool { get; private set; }
   public Dictionary<string, int> preconditionsInt { get; private set; }
    public delegate bool PreConditionsDelegate(GOAPState state);
    public List<PreConditionsDelegate> preConditions;

    public delegate void EffectDelegate(GOAPState state);
    public List<EffectDelegate> effects;

    public Dictionary<string, bool> effectsBool { get; private set; }
    public Dictionary<string, int> effectsInt { get; private set; }
    public string name { get; private set; }
    public float cost { get; private set; }

    public GOAPAction(string name)
    {
        this.name = name;
        cost = 1f;
        preconditionsBool = new Dictionary<string, bool>();
        preconditionsInt = new Dictionary<string, int>();
        preConditions= new List<PreConditionsDelegate>();

        effectsBool = new Dictionary<string, bool>();
        effectsInt = new Dictionary<string, int>();
        effects = new List<EffectDelegate> ();
    }

    public GOAPAction Cost(float cost)
    {
        if (cost < 1f)
        {
            //Costs < 1f make the heuristic non-admissible. h() could overestimate and create sub-optimal results.
            //https://en.wikipedia.org/wiki/A*_search_algorithm#Properties
            Debug.Log(string.Format("Warning: Using cost < 1f for '{0}' could yield sub-optimal results", name));
        }
        this.cost = cost;
        return this;
    }
    public GOAPAction Pre(string s, bool value)
    {
        preconditionsBool[s] = value;
        return this;
    }


    public GOAPAction Pre(PreConditionsDelegate f)
    {
        preConditions.Add(f);
        return this;
    }


    public GOAPAction Pre(string s, int value)
    {
        preconditionsInt[s] = value;
        return this;
    }
    public GOAPAction Effect(string s, bool value)
    {
        effectsBool[s] = value;
        return this;
    }

    public GOAPAction Effect(string s, int value)
    {
        effectsInt[s] = value;
        return this;
    }

    public GOAPAction Effect(EffectDelegate f)
    {
        effects.Add(f);
        return this;
    }

    public GOAPAction AddEffect(string s, int value)
    {
        if (effectsInt.ContainsKey(s)) {
            effectsInt[s] += value;
        }
        effectsInt[s] = value;
        return this;
    }

}
