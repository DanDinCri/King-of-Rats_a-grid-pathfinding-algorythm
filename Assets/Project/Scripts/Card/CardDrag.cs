using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;


public class CardDrag : MonoBehaviour {
    public PlayerCard card;

    public Camera cam;

    public Vector2 gridPos;
    public Vector2 lastGridPos;

    public float countBack;
    public float countBackMax = 1;

    public bool selected;
    public bool released = false;
    public bool isOn;
    [SerializeField]
    private float backSpeed = 10;
    [SerializeField]
    private float moveSpeed = 10;

    public Vector2 initialCardPosition = new Vector2(0, 0);

    public event Action<int> Interacted = delegate { };


    private void Awake() {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void FixedUpdate() {
        if (!GameManager.instance.playerDecided && !GameManager.instance.playerDeciding
            && !card.used && GameManager.instance.player.actualPA >= card.cardCost) {
            released = false;

            if (!selected) {
                BackToPosition();
                if (!isOn) Interacted(0);
                else Interacted(1);
            } else if (selected && !released) {
                CardActionHold();
                Interacted(2);
            }
        } else {
            BackToPosition();
            isOn = false;
            Interacted(0);
        }
    }

    public void CardActionHold() {
        if (selected && !released) {
            Vector3 tmp = cam.ScreenToWorldPoint(Input.mousePosition);
            gridPos = GridManager.instance.CheckNearestNode(tmp);

            if (lastGridPos != gridPos && !GridManager.instance.CheckNodeOccupied(gridPos)) {
                lastGridPos = gridPos;
                GridManager.instance.CleanEndPath();
                countBack = 0;
            } else {
                if (GridManager.instance.CheckNodeOccupied(gridPos)) GridManager.instance.CleanEndPath();
                else {
                    if (countBack < countBackMax) countBack += 0.1f * 100 * Time.deltaTime;
                    else if (countBack > countBackMax) {
                        countBack = countBackMax;
                        card.CardActionHold(gridPos);
                    }
                }
            }
        }
    }
    public void CardActionRelease() {
        if (!selected && released) {
            card.CardActionRelease(gridPos);
        }

    }

    private void OnMouseOver() {
        if (!GameManager.instance.playerDecided && !GameManager.instance.playerDeciding
            && !card.used && GameManager.instance.player.actualPA >= card.cardCost) {
            if (!selected) isOn = true;
            else isOn = false;
            if (Input.GetMouseButtonDown(0)) {
                selected = true;

            }
            if (Input.GetMouseButtonUp(0)) {
                selected = false;
                //BackToPosition();
            }
        }
    }

    private void OnMouseExit() {
        if (!selected) isOn = false;
    }

    private void OnMouseUp() {
        if (!GameManager.instance.playerDecided && !GameManager.instance.playerDeciding
            && !card.used && GameManager.instance.player.actualPA >= card.cardCost) {
            selected = false;
            released = true;
            isOn = false;
            CardActionRelease();
            //BackToPosition();
        }
    }

    void OnMouseDrag() {
        if (!GameManager.instance.playerDecided && !GameManager.instance.playerDeciding
            && !card.used && GameManager.instance.player.actualPA >= card.cardCost) {
            selected = true;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed * Time.deltaTime);
        }
    }

    private void BackToPosition() {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, initialCardPosition, backSpeed * Time.deltaTime);
    }

    public void UpdateInitialPos(Vector2 newPos) {
        initialCardPosition = newPos;
    }


}
