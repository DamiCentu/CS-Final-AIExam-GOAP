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
    public int life = 0;
    public void Start()
    {
        life = 0;
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
        life -= normalDamage;
        if (life < 0) {
            int extra_damage = life;
            life = 0;
            return extra_damage;
        }
        else return 0;
    }

    public void Create() {
        modelDefense.SetActive(true);
        life = life_defense;
        _modelDefenseCollider.enabled = true;
        EventsManager.TriggerEvent(EventsConstants.IA_GET_DEFENSE, new object[] { life_defense });
    }


    internal void Upgrade()
    {
        life = life_defense_update;
        modelDefense.SetActive(false);
        upgradeDefense.SetActive(true);
        EventsManager.TriggerEvent(EventsConstants.IA_GET_DEFENSE, new object[] { life_defense_update });
    }
}
