using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    public GameObject cannonPrefab;
    public GameObject cannonPrefabUpgrade;
    public GameObject target;

    public void Start()
    {
        cannonPrefab.SetActive(false);
        cannonPrefabUpgrade.SetActive(false);
        var rotDir = target.transform.position - transform.position;
        this.transform.rotation = Quaternion.LookRotation(rotDir);
    }
    
    public void Activate() {
        cannonPrefab.SetActive(true);
    }

    public void Upgrade()
    {
        cannonPrefab.SetActive(false);
        cannonPrefabUpgrade.SetActive(true);
    }

    public void AttackNormal()
    {
        Debug.Log("PIUMMMMM");
    }
    public void AttackSpecial()
    {
        Debug.Log("PIUMMMMM SPECIALLL");
    }
}
