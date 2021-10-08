using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //variables
    public int health;
    public int attackDamage;
    public bool isDead;
    public Rigidbody rb;
    public GameObject homeBase;
    public bool crouching = false;
    protected float distanceBetweenHomeBase;
    protected int healingCoolDown = 2000;
    protected int maxHealCoolDown;
    protected float actionDistance = 5.5f;
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
    //take damage method that freezes character if they die
    public void TakeDamage(int damage)
    {
        health = health - damage;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    //healing method.......
    public void Heal(int hp)
    {
        health = health + hp;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }
}
