using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleClickToMove : MonoBehaviour {
    public Camera cam;

    public Creature player;

    public Vector2 gridPos;
    public Vector2 lastGridPos;


    void Update() {
        if (!GameManager.instance.playerDecided) {
            Vector3 tmp = cam.ScreenToWorldPoint(Input.mousePosition);
            gridPos = GridManager.instance.CheckNearestNode(tmp);

            if (lastGridPos != gridPos) {
                lastGridPos = gridPos;
                if (!GridManager.instance.calculating && !player.moving) {
                    player.targetPos = gridPos;
                    player.SendPos();
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && !GridManager.instance.calculating && !player.moving) {
                player.targetPos = gridPos;
                player.SendPosAndMove();
            }
        }
    }

}
