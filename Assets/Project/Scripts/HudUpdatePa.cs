using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudUpdatePa : MonoBehaviour {
    public TextMeshProUGUI text;


    void Start() {
        GameManager.instance.player.PaUpdated += UpdateText;
        UpdateText(GameManager.instance.player.actualPA);
    }

    void OnDisable() {
        GameManager.instance.player.PaUpdated -= UpdateText;
    }

    public void UpdateText(int actualPa) {
        text.text = "PA - " + actualPa;
    }

}
