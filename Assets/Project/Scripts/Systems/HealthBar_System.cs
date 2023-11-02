using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar_System : HealthSystem
{
    public Sprite[] healthBarSprites;
    public SpriteRenderer healthBarRenderer;
    int sprite;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        health = maxHealth;
        sprite = (int)QuartDeVida(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { 
            health -= 10;
            sprite = (int)QuartDeVida(health);
        }
        if (health <= 0)
        {
            healthBarRenderer.enabled = false;
        }
        else
        {
            if (sprite >= 0)
                healthBarRenderer.sprite = healthBarSprites[sprite];
        }
        
    }

    public override void Death()
    {
    }

    float QuartDeVida(float x)
    {
        float j = (((x/maxHealth)*100)+12.5f)/25;
        x = (int)Math.Truncate(j);
        return x-1;
    }
}
