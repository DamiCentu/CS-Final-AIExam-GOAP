using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Defense : MonoBehaviour {

    public GameObject modelDefense;
    public int life_defense=100;
    public int life_defense_update=175;
    Collider _modelDefenseCollider;
    public int life = 0;
    public Text text;

    public void Start()
    {
        life = 0;
        // _modelDefenseCollider = modelDefense.GetComponent<Collider>();
        _modelDefenseCollider = GetComponent<Collider>();
        if (!_modelDefenseCollider)
            throw new Exception("ModelDefenseCollider null");

        modelDefense.SetActive(false);
      //  _modelDefenseCollider.enabled = false;
    }

    internal int ReceiveDamage(int normalDamage)
    {
        life -= normalDamage;
        text.text = life.ToString();
        if (life <= 0) {
            modelDefense.SetActive(false);
            int extra_damage = life;
            life = 0;
            text.text = "";
            return extra_damage;
        }

        else return 0;
    }

    public void Create() {
        modelDefense.SetActive(true);
        life = life_defense;
        text.text = life.ToString();
        _modelDefenseCollider.enabled = true;
        Item i = this.GetComponent<Item>();
        if (i.owner == "ia")
            EventsManager.TriggerEvent(EventsConstants.IA_GET_DEFENSE, new object[] { life_defense });
    }


    internal void Upgrade()
    {
        life = life_defense_update;
        text.text = life.ToString();
        modelDefense.SetActive(true);
        Item i = this.GetComponent<Item>();
        if (i.owner == "ia")
            EventsManager.TriggerEvent(EventsConstants.IA_GET_DEFENSE, new object[] { life_defense_update });
    }
}
