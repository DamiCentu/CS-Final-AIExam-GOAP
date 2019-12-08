using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateableTextAbstract : MonoBehaviour {

    protected Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        if (!text)
            throw new System.Exception("Text null");
    }

    protected void SetText(string textString)
    {
        if(text)
            text.text = textString;
    }
}
