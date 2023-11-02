using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature {
    public float attackDist = 1;//TEMPORAL (sustituirlo por la Distancia de las Card)

    public void AiDecide() {
        Debug.Log("***AI deciding...");
        //TODO: Cecision de Casilla por Card y Conducta

        targetPos = GridManager.instance.GetNodeOccupiedByPlayer();

        //TODO: la Distancia se basara en Card y Conducta (attackDist es temporal):
        targetPos = GridManager.instance.FindOptimizedPos(pos, targetPos, attackDist);

        SendPosAndMove();
        Debug.Log("***AI complete");
    }

}
