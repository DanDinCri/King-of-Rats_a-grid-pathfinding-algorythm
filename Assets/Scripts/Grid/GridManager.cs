using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;


public enum CellContentType {
    EMPTY,
    CREATURE,
    PLAYER,
    OBSTACLE
}

[Serializable]
public class GridNode {
    public GameObject node;
    public CellContentType cellCC;
    public CellContent cellC;
    //public bool occupied;
}

public class GridManager : MonoBehaviour {
    public static GridManager instance;

    public List<GridNode> gridNodes;
    public List<Vector2> gridNPos;

    public int xSize;
    public int ySize;

    public float occupiedChance = 20;

    public GameObject node;

    public List<Vector2> endPath;

    public bool calculating;

    public bool debugPrint;

    public Color transparentCellC;
    public Color highlightCellC;


    void Awake() {
        if (instance == null) instance = this;
        else Destroy(instance);

        CalculateGrid();
    }

    public void CalculateGrid() {
        if (endPath != null) endPath.Clear();
        endPath = new List<Vector2>();
        if (gridNodes != null) gridNodes.Clear();
        gridNodes = new List<GridNode>();
        if (gridNPos != null) gridNPos.Clear();
        gridNPos = new List<Vector2>();

        for (int i = 0; i < xSize; ++i) {
            for (int j = 0; j < ySize; ++j) {
                GridNode tmp= new GridNode();
                tmp.node = Instantiate(node, new Vector2(transform.position.x+i, transform.position.y + j), transform.rotation);
                tmp.node.transform.parent = this.transform;
                int poss = UnityEngine.Random.Range(0,100);
                if (poss < occupiedChance) tmp.cellCC = CellContentType.OBSTACLE;
                //instanciar obstaculo (prepararlo)
                gridNodes.Add(tmp);
                gridNPos.Add(new Vector2(i, j));
                if (debugPrint && gridNodes[gridNodes.Count - 1].node.transform.GetChild(0).GetComponent<TextMeshPro>()) {
                    gridNodes[gridNodes.Count - 1].node.transform.GetChild(0).GetComponent<TextMeshPro>().text = "x:" + i + "-y:" + j;
                } else if (gridNodes[gridNodes.Count - 1].node.transform.GetChild(0).GetComponent<TextMeshPro>()) {
                    gridNodes[gridNodes.Count - 1].node.transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
                }
            }
        }
        PaintNodeColors();
    }
    public void RealculateGrid() {
        if (endPath != null) endPath.Clear();
        endPath = new List<Vector2>();

        for (int i = 0; i < xSize; ++i) {
            for (int j = 0; j < ySize; ++j) {
                if (!CheckNodeOccupiedByCreature(new Vector2(i, j))
                    && !CheckNodeOccupiedByPlayer(new Vector2(i,j))) {
                    int poss = UnityEngine.Random.Range(0, 100);
                    if (poss < occupiedChance) SetNodeContent(new Vector2(i, j), null, CellContentType.OBSTACLE);
                }
            }
        }
        PaintNodeColors();
    }

    public void CalculatePathRecursiveCall(Vector2 init, Vector2 target) {
        if (endPath != null) endPath.Clear();
        endPath = new List<Vector2>();
        calculating = true;
        if (!CheckNodeOccupied(target)) CalculatePath(init, target, null);
        PaintNodeColors();
        calculating = false;
    }

    public void CalculatePath(Vector2 init, Vector2 target, List<Vector2> exploredCells) {

        //lockEnd.WaitOne();
        if (calculating) {
            List<Vector2> exploredC = new List<Vector2>();
            if (exploredCells != null) {
                foreach (Vector2 cell in exploredCells) {
                    exploredC.Add(cell);
                }
            }
            exploredC.Add(init);

            if (init == target) {//si llega al destino
                if (endPath.Count() == 0 || exploredC.Count() < endPath.Count())
                {
                    endPath = exploredC;
                }
                return;
            } else {//si aun no ha llegado
                if (endPath.Count() != 0 && exploredC.Count() >= endPath.Count())
                {
                    return;
                }
            }

            List<Vector2> neighbors = new List<Vector2>();

            if (init.x > 0) {
                Vector2 actualP = init;
                actualP.x--;
                List<Vector2> explored = exploredC;
                if (!CheckNodeOccupied(actualP) && !explored.Contains(actualP)) {
                    neighbors.Add(actualP);
                }
            }
            if (init.x < xSize - 1) {
                Vector2 actualP = init;
                actualP.x++;
                List<Vector2> explored = exploredC;
                if (!CheckNodeOccupied(actualP) && !explored.Contains(actualP)) {
                    neighbors.Add(actualP);
                }
            }
            if (init.y > 0) {
                Vector2 actualP = init;
                actualP.y--;
                List<Vector2> explored = exploredC;
                if (!CheckNodeOccupied(actualP) && !explored.Contains(actualP)) {
                    neighbors.Add(actualP);
                }
            }
            if (init.y < ySize - 1) {
                Vector2 actualP = init;
                actualP.y++;
                List<Vector2> explored = exploredC;
                if (!CheckNodeOccupied(actualP) && !explored.Contains(actualP)) {
                    //CalculatePath(actualP, target, explored);
                    neighbors.Add(actualP);
                }
            }

            neighbors.Sort(delegate (Vector2 a, Vector2 b)//hace Sort por distancia (asi empezara a buscar por la direccion mas optima)
            {
                return (Vector2.Distance(a,target).CompareTo(Vector2.Distance(b,target)));
            });

            foreach(Vector2 neighbor in neighbors)
            {
                List<Vector2> explored = exploredC;
                CalculatePath(neighbor, target, explored);
            }

        } else return;

        //lockEnd.ReleaseMutex();
    }

    public List<UnityEngine.Transform> GetPath(int maxSteps) {
        List<UnityEngine.Transform> tmp = new List<UnityEngine.Transform>();
        int steps = 0;
        for (int i = 0; i < endPath.Count && steps <= maxSteps; ++i, ++steps) {
            tmp.Add(GetNodeByPos(endPath[i]).transform);
        }
        return tmp;
    }

    public Vector2 GetLastPos(int maxSteps) {
        if (maxSteps > endPath.Count() - 1) maxSteps = endPath.Count() - 1;
        while (endPath.Count()-1 > maxSteps) endPath.RemoveAt(endPath.Count()-1);
        PaintNodeColors();
        if (maxSteps > 0) return endPath[maxSteps];
        else return new Vector2(-1,-1);
    }

    public UnityEngine.Transform GetGridPos(Vector2 pos) {
        if (gridNPos.Contains(pos)) return GetNodeByPos(pos).transform;
        else return null;
    }
    public GameObject GetNodeByPos(Vector2 pos) {
        int index = gridNPos.IndexOf(pos);
        return gridNodes[index].node;
    }

    public void SetNodeContent(Vector2 pos, CellContent cellC, CellContentType cellCC) {
        int index = gridNPos.IndexOf(pos);
        gridNodes[index].cellC = cellC;
        gridNodes[index].cellCC = cellCC;
    }
    public CellContent GetNodeContent(Vector2 pos) {
        int index = gridNPos.IndexOf(pos);
        return gridNodes[index].cellC;
    }

    public bool CheckNodeOccupied(Vector2 pos) {
        int index = gridNPos.IndexOf(pos);
        return gridNodes[index].cellCC != CellContentType.EMPTY;
    }
    public bool CheckNodeOccupiedByCreature(Vector2 pos) {
        int index = gridNPos.IndexOf(pos);
        return gridNodes[index].cellCC == CellContentType.CREATURE;
    }
    public bool CheckNodeOccupiedByPlayer(Vector2 pos) {
        int index = gridNPos.IndexOf(pos);
        return gridNodes[index].cellCC == CellContentType.PLAYER;
    }

    public Vector2 GetNodeOccupiedByPlayer() {
        Vector2 tmp = new Vector2(-1, -1);
        for (int i = 0; i < gridNodes.Count; ++i) {
            if (gridNodes[i].cellCC == CellContentType.PLAYER) tmp = gridNPos[i];
        }
        Debug.Log("***Found player");
        return tmp;
    }

    public Vector2 CheckNearestNode(Vector2 pos) {
        float dist = 100000;
        Vector2 tmp = new Vector2();
        for (int i = 0; i < gridNPos.Count; ++i) {
            if (Vector2.Distance(GetNodeByPos(gridNPos[i]).transform.position, pos) < dist) {
                tmp = gridNPos[i];
                dist = Vector2.Distance(GetNodeByPos(gridNPos[i]).transform.position, pos);
            }
        }
        return tmp;
    }

    public List<Enemy> GetRadiusEnemy(Vector2 pos, int radius) {
        List<Enemy> tmp = new List<Enemy>();

        for (int i = 0; i < radius; ++i) {
            for (int j = 0; j < radius; ++j) {
                if (i > 0 && i < xSize - 1 && j > 0 && j < ySize) {
                    Vector2 actualPos = new Vector2(i, j);
                    if (!CheckNodeOccupied(actualPos) && CheckNodeOccupiedByCreature(actualPos)) {
                        tmp.Add((Enemy)GetNodeContent(actualPos));
                    }
                }
            }
        }

        return tmp;
    }
    public List<Player> GetRadiusPlayer(Vector2 pos, int radius) {
        List<Player> tmp = new List<Player>();

        for (int i = 0; i < radius; ++i) {
            for (int j = 0; j < radius; ++j) {
                if (i > 0 && i < xSize - 1 && j > 0 && j < ySize) {
                    Vector2 actualPos = new Vector2(i, j);
                    if (!CheckNodeOccupied(actualPos) && CheckNodeOccupiedByPlayer(actualPos)) {
                        tmp.Add((Player)GetNodeContent(actualPos));
                    }
                }
            }
        }

        return tmp;
    }

    public void PaintNodeColors() {
        if (debugPrint) {
            for (int i = 0; i < gridNodes.Count; ++i) {
                if (gridNodes[i].cellCC == CellContentType.OBSTACLE)
                    gridNodes[i].node.GetComponent<SpriteRenderer>().color = Color.red;
                else gridNodes[i].node.GetComponent<SpriteRenderer>().color = Color.white;
                if (endPath != null)
                    if (endPath.Contains(gridNPos[i]))
                        gridNodes[i].node.GetComponent<SpriteRenderer>().color = Color.green;
            }
        } else {
            for (int i = 0; i < gridNodes.Count; ++i) {
                gridNodes[i].node.GetComponent<SpriteRenderer>().color = transparentCellC;
                if (endPath != null)
                    if (endPath.Contains(gridNPos[i]))
                        gridNodes[i].node.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    public void PaintCertainNodeColors() {
        if (debugPrint) {
            for (int i = 0; i < gridNodes.Count; ++i) {
                if (gridNodes[i].cellCC == CellContentType.OBSTACLE)
                    gridNodes[i].node.GetComponent<SpriteRenderer>().color = Color.red;
                else gridNodes[i].node.GetComponent<SpriteRenderer>().color = Color.white;
                if (endPath != null)
                    if (endPath.Contains(gridNPos[i]))
                        gridNodes[i].node.GetComponent<SpriteRenderer>().color = Color.green;
            }
        } else {
            for (int i = 0; i < gridNodes.Count; ++i) {
                gridNodes[i].node.GetComponent<SpriteRenderer>().color = transparentCellC;
                if (endPath != null)
                    if (endPath.Contains(gridNPos[i]))
                        gridNodes[i].node.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    public void CleanEndPath() {
        if (endPath != null) endPath.Clear();
        endPath = new List<Vector2>();
        PaintNodeColors();
    }

    public Vector2 FindOptimizedPos(Vector2 init, Vector2 target, float maxAttackDist) {
        float maxDistReached = 1000;//TODO: esto es de prueba

        if (Vector2.Distance(GetGridPos(init).position, GetGridPos(target).position) > maxAttackDist) {
            Vector2 possiblePos = new Vector2();
            foreach (Vector2 pos in gridNPos) {
                if (pos != target) {//si no es la casilla Target
                    //comprueba la casilla mas cercana respecto a Target
                    if (!CheckNodeOccupied(pos)
                        && Vector2.Distance(GetGridPos(pos).position, GetGridPos(target).position) <= maxAttackDist
                        && Vector2.Distance(GetGridPos(init).position, GetGridPos(pos).position) < maxDistReached) {
                        maxDistReached = Vector2.Distance(GetGridPos(init).position, GetGridPos(pos).position);
                        possiblePos = pos;
                    }
                }
            }

            if (maxDistReached < 1000) {//TODO: esto es de prueba
                return possiblePos;
            }
        }

        return target;

    }

}
