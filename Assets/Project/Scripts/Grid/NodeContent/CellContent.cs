using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellContent : MonoBehaviour {
    public Vector2 pos;

    public virtual void Init(Vector2 pos) {
        this.pos = pos;
    }

}
