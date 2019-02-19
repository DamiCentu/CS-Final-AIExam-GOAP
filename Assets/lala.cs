using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lala : MonoBehaviour {

    public GameObject IA;
	void Start () {
        IA.GetComponent<Tipito>().shouldRePlan = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.L)) {
            IA.GetComponent<Tipito>().shouldRePlan = true;
        }
	}
}
