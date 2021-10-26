using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    //variables
    public float speed = 15;
    public int jumpHeight;
    public Text healthtxt;
    public GameObject gameOverCanvas;
    protected Enemy enemyScript;
    private float originSpeed;
    private bool canAttack;
    private float distanceBetweenTarget;
    // Start is called before the first frame update
    void Start()
    {
        //sets max varibles to current variables because everything starts with max stats
        maxHealth = health;
        //obviously don't want be dead or attacking when loaded into level
        isDead = false;
        canAttack = false;
        //Gets rigidbody from gameobject automatically so passing it into varible maunally is not needed
        rb = this.gameObject.GetComponent<Rigidbody>();
        originSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //distance between player and their home base (constantly calculated)
        distanceBetweenHomeBase = Vector3.Distance(homeBase.transform.position, this.transform.position);
        //updates health text
        healthtxt.text = health.ToString();
        //movement for player
        float Xaxis = Input.GetAxis("Horizontal") * speed;
        float Zaxis = Input.GetAxis("Vertical") * speed;
        Vector3 movePos = transform.right * Xaxis + transform.forward * Zaxis;
        Vector3 newMovePos = new Vector3(movePos.x, rb.velocity.y, movePos.z);
        rb.velocity = newMovePos;
        //if player is dead a gameover display becomes visable
        if (isDead == true) { gameOverCanvas.SetActive(true); }
        else
        {
            //if player is alive, game over display is not visable
            gameOverCanvas.SetActive(false);
            //healing mech that prevents rapid function
            if (distanceBetweenHomeBase <= actionDistance)
            {
                
                if (Time.time > nextHealCoolDown )
                {
                    //always heals a 4th of max health
                    Heal(maxHealth / 4);
                    nextHealCoolDown = Mathf.RoundToInt(Time.time) + healingCoolDown;
                }
            }

            if (target != null)
            {
                //will only be calculated if target is not null, to prevent null exception
                distanceBetweenTarget = Vector3.Distance(this.transform.position, target.transform.position);
                //if player is in range of target, player can attack target
                if (distanceBetweenTarget <= actionDistance)
                {
                    canAttack = true;
                }
                if (distanceBetweenTarget > actionDistance)
                {
                    canAttack = false;
                }
            }
            //when space is press and player is in range target will take damage
            if (Input.GetKeyDown(KeyCode.Space) && canAttack == true)
            {
                enemyScript.TakeDamage(this.attackDamage);
            }
            //run mech
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed = speed * 2;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed = originSpeed;
            }
            //crouch mech
            if (Input.GetKeyDown(KeyCode.C))
            {
                speed = speed / 3;
                //crouching makes player slightly smaller to simulate "crouching"
                this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x, this.gameObject.transform.localScale.y / 1.25f, this.gameObject.transform.localScale.z);
                crouching = true;
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                speed = originSpeed;
                this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x, this.gameObject.transform.localScale.y * 1.25f, this.gameObject.transform.localScale.z);
                crouching = false;
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        //if player enters trigger and it is not either a non=target or another player, they will be made players target
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag)
        {
            target = other.gameObject;
            enemyScript = target.GetComponent<Enemy>();
        }
    }
    public void OnCollisionEnter(Collision other)
    {
        //same with player bumbing into target
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag)
        {
            target = other.gameObject;
            enemyScript = target.GetComponent<Enemy>();
        }
    }

}
