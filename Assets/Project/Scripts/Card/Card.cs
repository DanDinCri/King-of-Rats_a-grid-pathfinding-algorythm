using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public abstract class Card : MonoBehaviour {
    public int cardCost;
    public int cardLevel;
    
    string cardName;

    //public List<Effect> effects;//TODO

    public bool used = false;

    public bool destroyOnUse;//TEMPORAL


    void Start() {
        GameManager.instance.RoundPassed += ResetCard;
    }
    void OnDisable() {
        GameManager.instance.RoundPassed -= ResetCard;
    }

    public void ResetCard() {
        used = false;
    }


}
