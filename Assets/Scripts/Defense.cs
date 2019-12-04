using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour {

    public GameObject modelDefense;
    public GameObject upgradeDefense;
    public int cost_defense;
    public int cost_defense_update;
    Collider _modelDefenseCollider;

    public void Start()
    {
        // _modelDefenseCollider = modelDefense.GetComponent<Collider>();
        _modelDefenseCollider = GetComponent<Collider>();
        if (!_modelDefenseCollider)
            throw new Exception("ModelDefenseCollider null");

        modelDefense.SetActive(false);
        upgradeDefense.SetActive(false);
        _modelDefenseCollider.enabled = false;
    }

    public Defense Create() {
        modelDefense.SetActive(true);
        return this;
    }

    public Defense Set()
    {
        _modelDefenseCollider.enabled = true;
        return this;
    }

    internal void Upgrade()
    {
        modelDefense.SetActive(false);
        upgradeDefense.SetActive(true);
    }
}
