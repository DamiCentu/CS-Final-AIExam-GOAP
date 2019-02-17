using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldTextBehaviour : MonoBehaviour {

    Text _text;
	void Awake () {
        _text = GetComponent<Text>();
        if (!_text)
            throw new System.Exception("Text null");

        EventsManager.SubscribeToEvent(EventsConstants.UI_UPDATE_GOLD, OnUpdateGoldUI);
        _text.text = "Gold: 0";
	}

    private void OnUpdateGoldUI(object[] parameterContainer)
    {
        _text.text = "Gold: " + ((int)parameterContainer[0]).ToString();
    }
}
