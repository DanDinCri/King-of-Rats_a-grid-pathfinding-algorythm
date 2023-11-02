using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Creature player;
    public bool playerDecided;//salta turno
    public bool playerDeciding;//bloquea la interaccion temporalmente
    [SerializeField]
    private List<Enemy> enemies;
    [SerializeField]
    private List<GameObject> possibleEnemies;

    public int maxEnemies = 1;

    public event Action RoundPassed = delegate { };
    public event Action<bool> PlayerDecidedUpdate = delegate { };
    public event Action<bool> PlayerDecidingUpdate = delegate { };


    void Awake() {
        if (instance == null) instance = this;
        else Destroy(instance);
    }

    void Start() {
        player = GameObject.Find("Player").GetComponent<Creature>();

        player.SetPosToGrid();

        RandomizeCreatures();

        StartCoroutine(GameLoop());
    }

    public void RandomizeCreatures() {
        for (int i=0; i < maxEnemies && enemies.Count< ((GridManager.instance.xSize / 2)-1) * (GridManager.instance.ySize-1); i++) {
            var tmp = Instantiate(possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Count)], transform.position, transform.rotation);
            bool done = false;
            do {
                tmp.GetComponent<Creature>().pos =
                    new Vector2(UnityEngine.Random.Range(GridManager.instance.xSize / 2, GridManager.instance.xSize),
                    UnityEngine.Random.Range(0, GridManager.instance.ySize - 1));
                if (!GridManager.instance.CheckNodeOccupied(tmp.GetComponent<Creature>().pos)) done = true; ;
            } while (!done);
            enemies.Add(tmp.GetComponent<Enemy>());
            GridManager.instance.SetNodeContent(enemies[i].pos, enemies[i], CellContentType.CREATURE);
        }

        GridManager.instance.RealculateGrid();//despues de randomizar criaturas

    }

    public void SetPlayerDecided(bool dec) {
        playerDecided = dec;
        PlayerDecidedUpdate(dec);
    }
    public void SetPlayerDeciding(bool dec) {
        playerDeciding = dec;
        PlayerDecidingUpdate(dec);
    }

    IEnumerator GameLoop() {
        Debug.Log("***GameLoop");
        //Player decide
        if (playerDecided) {
            GridManager.instance.CleanEndPath();
            yield return new WaitForSeconds(1);

            for(int i=0; i< enemies.Count; i++) {
                GridManager.instance.CleanEndPath();

                if (enemies[i] != null) {
                    enemies[i].AiDecide();
                    yield return new WaitForSeconds(2);
                }
                GridManager.instance.CleanEndPath();
                yield return new WaitForSeconds(1);
            }

            SetPlayerDecided(false);
            Debug.Log("***All enemies decided");

            RoundPassed();
        }

        yield return new WaitForSeconds(1);//o 2?

        StartCoroutine(GameLoop());
    }

}
