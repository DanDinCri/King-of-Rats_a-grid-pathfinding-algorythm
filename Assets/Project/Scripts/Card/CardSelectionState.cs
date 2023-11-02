using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;


public class CardSelectionState : MonoBehaviour {
    public CardDrag card;

    public Vector2 originalPos;
    public Vector2 originalScale;

    public Vector2 onPos;
    public Vector2 onScale;

    public Vector2 shrinkPos;
    public Vector2 shrinkScale;

    public float moveSpeed = 5;
    public float shrinkSpeed = 5;

    public List<SpriteRenderer> spr;
    public int layerBase;
    public int layerInteracting;


    void Start() {
        originalPos = transform.localPosition;
        originalScale = transform.localScale;
        card.Interacted += ShrinkCard;
    }
    void OnDisable() {
        card.Interacted -= ShrinkCard;
    }

    public void ShrinkCard(int state) {
        switch (state) {
            case 0://idle
                transform.localPosition = Vector2.Lerp(transform.localPosition, originalPos, moveSpeed * Time.deltaTime);
                transform.localScale = Vector2.Lerp(transform.localScale, originalScale, shrinkSpeed * Time.deltaTime);
                for (int i = 0; i < spr.Count; i++) {
                    spr[i].sortingOrder = layerBase*2 + spr.Count-i;
                }
                break;
            case 1://on
                transform.localPosition = Vector2.Lerp(transform.localPosition, onPos, moveSpeed * Time.deltaTime);
                transform.localScale = Vector2.Lerp(transform.localScale, onScale, shrinkSpeed * Time.deltaTime);
                for (int i = 0; i < spr.Count; i++) {
                    spr[i].sortingOrder = layerInteracting*2 + spr.Count-i;
                }
                break;
            case 2://drag
                transform.localPosition = Vector2.Lerp(transform.localPosition, shrinkPos, moveSpeed * Time.deltaTime);
                transform.localScale = Vector2.Lerp(transform.localScale, shrinkScale, shrinkSpeed * Time.deltaTime);
                for (int i = 0; i < spr.Count; i++) {
                    spr[i].sortingOrder = layerInteracting*2 + spr.Count-i;
                }
                break;
            default:

                break;
        }
    }

}
