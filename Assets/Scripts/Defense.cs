using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour {

    public GameObject modelDefense;
    public GameObject upgradeDefense;

    public void Start()
    {
        modelDefense.SetActive(false);
        upgradeDefense.SetActive(false);
    }

    public void Activate() {
        modelDefense.SetActive(true);
    }

    internal void Upgrade()
    {
        modelDefense.SetActive(false);
        upgradeDefense.SetActive(true);
    }
}
