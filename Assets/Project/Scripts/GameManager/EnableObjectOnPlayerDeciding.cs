using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnableObjectOnPlayerDeciding : MonoBehaviour {
    public bool inverse;
    public GameObject obj;


    void Start() {
        GameManager.instance.PlayerDecidingUpdate += EnableObj;
    }
    void OnDisable() {
        GameManager.instance.PlayerDecidingUpdate -= EnableObj;
    }

    public void EnableObj(bool dec) {
        if (inverse) dec = !dec;
        obj.SetActive(dec);
    }

}
