using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectOnPlayerDecide : MonoBehaviour {
    public bool inverse;
    public GameObject obj;


    void Start() {
        GameManager.instance.PlayerDecidedUpdate += EnableObj;
    }
    void OnDisable() {
        GameManager.instance.PlayerDecidedUpdate -= EnableObj;
    }

    public void EnableObj(bool dec) {
        if (inverse) dec = !dec;
        obj.SetActive(dec);
    }

}
