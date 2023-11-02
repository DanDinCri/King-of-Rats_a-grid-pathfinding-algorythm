using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardMallet : MonoBehaviour {
    public List<GameObject> cardBase;
    public List<CardSelectionState> cardSS;

    public int maxCards = 4;

    public float cardXPos = 1.5f;
    public float cardYPos = 0;


    void Start() {
        ReorderCards();
        GameManager.instance.RoundPassed += ReorderCards;
    }
    void OnDisable() {
        GameManager.instance.RoundPassed -= ReorderCards;
    }

    public void ReorderCards() {
        cardBase = new List<GameObject>();
        cardSS = new List<CardSelectionState>();

        for (int i = 0; i < transform.childCount; i++) {//actualizar Cards actuales
            cardBase.Add(transform.GetChild(i).gameObject);
            cardSS.Add(cardBase[i].transform.GetChild(0).GetComponent<CardSelectionState>());
        }

        for (int i = 0; i < cardSS.Count; i++) {//asignar OrderLayer
            cardSS[i].layerBase = (cardSS.Count - i);
            cardSS[i].layerInteracting = cardSS.Count + 1;
        }

        for (int i = 0; i < cardBase.Count; i++) {
            if (i < maxCards) {
                cardBase[i].GetComponent<CardDrag>().UpdateInitialPos(
                    new Vector2(1 + i * cardXPos, cardYPos * i));
            } else {
                cardBase[i].GetComponent<CardDrag>().UpdateInitialPos(
                    new Vector2(cardXPos + maxCards/2, -5));

            }
        }
    }

}
