using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Creature : CellContent {
    public Vector2 targetPos;

    public int maxSteps = 3;

    public int actualPA;//Puntos de Accion
    public int maxPA = 3;

    public List<Transform> actualPath;

    //public Vector2 startPos;//Solo para testeos

    public bool moving = false;

    public float moveSpeed = 5;
    public float moveInterval = 0.5f;

    public event Action<int> PaUpdated = delegate { };


    void Start() {
        //startPos = pos;
        actualPA = maxPA;
        GameManager.instance.RoundPassed += ResetCreaturePA;
        SetPosToGrid();
    }

    void OnDisable() {
        GameManager.instance.RoundPassed -= ResetCreaturePA;
    }

    public void ResetCreaturePA() {
        actualPA = maxPA;
        PaUpdated(actualPA);
    }

    public void AddCreaturePA(int inc) {
        actualPA += inc;
        if (actualPA < 0) actualPA = 0;//no deberia darse este caso
        PaUpdated(actualPA);
    }

    public virtual void SetPosToGrid() {
        transform.position = GridManager.instance.GetGridPos(pos).position;
        GridManager.instance.SetNodeContent(pos, this, CellContentType.CREATURE);
    }
    /*public void ResetPlayerPos() {
        pos = startPos;
        SetPosToGrid();
    }*/

    public void SendPos() {//preview
        GridManager.instance.CalculatePathRecursiveCall(pos, targetPos);
        targetPos = GridManager.instance.GetLastPos(maxSteps);
    }
    public void SendPosAndMove() {//preview+move
        //actualPath = GridManager.instance.CalculatePath(initPos, targetPos, null);
        GridManager.instance.CalculatePathRecursiveCall(pos, targetPos);
        Move();//TODO: dejarlo aqui fijo?
    }

    void FixedUpdate() {
        if (moving)
            transform.position = Vector2.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
    }

    public void Move() {
        if(GridManager.instance.GetLastPos(maxSteps)!=new Vector2(-1,-1)
            && GridManager.instance.endPath.Count > 0)
            targetPos = GridManager.instance.GetLastPos(maxSteps);
            actualPath = GridManager.instance.GetPath(maxSteps);

            if (actualPath.Count() > 0) {
                moving = true;
                StartCoroutine(Moving());
            }
    }

    protected virtual IEnumerator Moving() {
        int steps = 0;
        GridManager.instance.SetNodeContent(pos, null, CellContentType.EMPTY);
        for (int i = 0; i < actualPath.Count && steps <= maxSteps; ++i, ++steps) {
            pos = actualPath[i].position;
            yield return new WaitForSeconds(moveInterval);
        }
        GridManager.instance.SetNodeContent(targetPos, this, CellContentType.CREATURE);
        pos = targetPos;
        moving = false;
    }

}
