using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IParalizado: MonoBehaviour
{
    //For x turns the affected can't move from its position
    bool canMove = false;

    public virtual int TurnsParalized(int turns) { return turns; }
}
