using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthSystem : MonoBehaviour
{
    protected int health;
    protected int maxHealth;
    public abstract void Death();
}
