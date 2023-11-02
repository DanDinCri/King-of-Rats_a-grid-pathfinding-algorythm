using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Player : Creature {
    public override void SetPosToGrid() {
        transform.position = GridManager.instance.GetGridPos(pos).position;
        GridManager.instance.SetNodeContent(pos, this, CellContentType.PLAYER);
    }

    protected override IEnumerator Moving() {
        int steps = 0;
        GridManager.instance.SetNodeContent(pos, null, CellContentType.EMPTY);
        for (int i = 0; i < actualPath.Count && steps <= maxSteps; ++i, ++steps) {
            pos = actualPath[i].position;
            yield return new WaitForSeconds(moveInterval);
        }
        GridManager.instance.SetNodeContent(targetPos, this, CellContentType.PLAYER);
        pos = targetPos;
        moving = false;

        GridManager.instance.CleanEndPath();
        GameManager.instance.SetPlayerDeciding(false);
    }

}
