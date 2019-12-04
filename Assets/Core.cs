using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour {
    int _life;
    public int default_life=100;
    public Text text;
	void Start () {
        _life = default_life;
	}
	

    internal void ReceiveDamage(int normalDamage)
    {
        _life -= normalDamage;
        text.text = _life.ToString();
    }
}
