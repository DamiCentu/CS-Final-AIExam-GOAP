using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Core : MonoBehaviour {
    int _life;
    public int default_life=100;
    public Text text;
    public Defense defense;
    public GameObject bulletPrefab;

	void Start () {
        _life = default_life;
        EventsManager.SubscribeToEvent(EventsConstants.IA_GET_DEFENSE, UpdateIaLife );
    }

    private void UpdateIaLife(object[] parameterContainer)
    {

        text.text = (_life ).ToString();
    }

    internal void ReceiveDamage(Vector3 cannonPos ,int normalDamage)
    {
        MakeBulletAnimation(cannonPos , defense.life > 0 ? defense.transform.position : transform.position);
        int damage_left = defense.ReceiveDamage(normalDamage);
        if(damage_left<0)_life += damage_left;
        text.text = (_life ).ToString();
        Item i = this.GetComponent<Item>();
        if (i.owner == "ia")
            EventsManager.TriggerEvent(EventsConstants.IA_IS_BEING_ATTACK, new object[] { (_life + defense.life) });

        if(_life <= 0)
        {
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { false });
            StartCoroutine(EndRoutine(i.owner));
        }
    }

    IEnumerator EndRoutine(string owner)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(owner != "ia" ? "GOAPWin" : "PlayerWin");  
    }

    internal float GetLife()
    {
        return _life + defense.life;
    }

    void MakeBulletAnimation(Vector3 spawn, Vector3 targetPos)
    {
        Instantiate(bulletPrefab, spawn, Quaternion.identity).GetComponent<Bullet>().SetBullet(targetPos);
    }
}
