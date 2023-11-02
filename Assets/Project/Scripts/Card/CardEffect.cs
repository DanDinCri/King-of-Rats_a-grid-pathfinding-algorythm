using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardEffect : PlayerCard {
    public override void CardActionHold(Vector2 gridPos) {
        //TODO: if no esta ocupada && esta a una distancia optima,
        //highlight de las casillas afectadas

        //GetRadiusEnemy(gridPos);
    }
    public override void CardActionRelease(Vector2 gridPos) {
        //TODO: if no esta ocupada && esta a una distancia optima,
        //ataque

        //GetRadiusEnemy(gridPos);
    }

}
