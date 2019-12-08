using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float speed = 5;
    public float distanceToDisappear = 1;

    Vector3 _targetPos;

    public void SetBullet(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }

	void Update () {
        if (_targetPos == null)
            return;

        var dir = (_targetPos - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(_targetPos, transform.position) < distanceToDisappear)
            Destroy(gameObject);
	}
}
