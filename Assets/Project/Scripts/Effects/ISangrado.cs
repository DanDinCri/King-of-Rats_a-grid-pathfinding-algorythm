using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISangrado : MonoBehaviour
{
    //Deal reduced damage for the next x turns
    private int sangrado = 5;
    private int turns = 0;

    public int SangradoDamage()
    {
        return sangrado;
    }
}
