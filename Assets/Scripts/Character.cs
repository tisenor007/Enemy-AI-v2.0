using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int health;
    public int attackDamage;
    public bool isDead;
    protected float actionDistance = 2.5f;
    protected int attackSpeed;
    protected GameObject target;
    protected int maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int damage)
    {
        health = health - damage;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
        }
    }
}
