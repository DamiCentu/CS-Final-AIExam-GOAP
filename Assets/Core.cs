using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour {
    int _life;
    public int default_life=100;
    public Text text;
    public Defense defense;
	void Start () {
        _life = default_life;
	}
	

    internal void ReceiveDamage(int normalDamage)
    {
        int damage_left =defense.ReceiveDamage(normalDamage);
        _life -= damage_left;
        text.text = (_life + defense.life_defense).ToString();
        EventsManager.TriggerEvent(EventsConstants.IA_IS_BEING_ATTACK, new object[] { (_life + defense.life_defense) });
    }
}
