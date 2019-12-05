using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour {

    public GameObject modelDefense;
    public GameObject upgradeDefense;
    public int life_defense=100;
    public int life_defense_update=175;
    Collider _modelDefenseCollider;
    private int _life;
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

    internal int ReceiveDamage(int normalDamage)
    {
        _life -= normalDamage;
        if (_life < 0) {
            int extra_damage = _life;
            _life = 0;
            return extra_damage;
        }
        else return 0;
    }

    public void Create() {
        modelDefense.SetActive(true);
        _life = life_defense;
        _modelDefenseCollider.enabled = true;
        EventsManager.TriggerEvent(EventsConstants.IA_GET_DEFENSE, new object[] { life_defense });
    }


    internal void Upgrade()
    {
        _life = life_defense_update;
        modelDefense.SetActive(false);
        upgradeDefense.SetActive(true);
        EventsManager.TriggerEvent(EventsConstants.IA_GET_DEFENSE, new object[] { life_defense_update });
    }
}
