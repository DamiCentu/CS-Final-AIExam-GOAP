using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    public GameObject cannonPrefab;
    public GameObject cannonPrefabUpgrade;
    public Transform target;

    public void Start()
    {
        cannonPrefab.SetActive(false);
        cannonPrefabUpgrade.SetActive(false);
        var rotDir = target.position - transform.position;
        this.transform.rotation = Quaternion.LookRotation(rotDir);
    }
    
    public void Create() {
        cannonPrefab.SetActive(true);
    }

    public void Upgrade()
    {
        cannonPrefab.SetActive(false);
        cannonPrefabUpgrade.SetActive(true);
    }

    public void Shoot()
    {
        cannonPrefab.SetActive(false);
        cannonPrefabUpgrade.SetActive(true);
    }
}
