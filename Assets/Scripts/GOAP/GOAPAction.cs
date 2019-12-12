using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPAction
{

    public delegate bool PreConditionsDelegate(WorldSpace w);
    public List<PreConditionsDelegate> preConditions;

    public delegate void EffectDelegate(WorldSpace w);
    public List<EffectDelegate> effects;


    public string name { get; private set; }
    public float cost { get; private set; }

    public GOAPAction(string name)
    {
        this.name = name;
        cost = 1f;
        this.preConditions = new List<PreConditionsDelegate>();
        this.effects = new List<EffectDelegate>();
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


    public GOAPAction Pre(PreConditionsDelegate f)
    {
        preConditions.Add(f);
        return this;
    }
    

    public GOAPAction Effect(EffectDelegate f)
    {
        effects.Add(f);
        return this;
    }


}
