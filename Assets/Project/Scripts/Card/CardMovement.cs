using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardMovement : PlayerCard {
    public override void CardActionHold(Vector2 gridPos) {
        if (!GridManager.instance.calculating && !GameManager.instance.player.moving) {
            GameManager.instance.player.targetPos = gridPos;
            GameManager.instance.player.SendPos();
        }
    }
    public override void CardActionRelease(Vector2 gridPos) {
        if (!GridManager.instance.CheckNodeOccupied(gridPos) && GridManager.instance.endPath.Count>0) {
            used = true;
            GameManager.instance.player.targetPos = gridPos;
            GameManager.instance.player.SendPosAndMove();
            GameManager.instance.SetPlayerDeciding(true);
            GameManager.instance.player.AddCreaturePA(-cardCost);
            if (destroyOnUse) Destroy(this.gameObject);
        }

    }

}
