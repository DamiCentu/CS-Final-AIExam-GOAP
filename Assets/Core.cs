using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {
    int _life;
    public int default_life=100;
	// Use this for initialization
	void Start () {
        _life = default_life;
	}
	

    internal void ReceiveDamage(int normalDamage)
    {
        _life -= normalDamage;
    }
}
