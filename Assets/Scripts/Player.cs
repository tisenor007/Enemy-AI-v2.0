using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
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
        maxHealCoolDown = healingCoolDown;
        maxHealth = health;
        isDead = false;
        canAttack = false;
        rb = this.gameObject.GetComponent<Rigidbody>();
        originSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        distanceBetweenHomeBase = Vector3.Distance(homeBase.transform.position, this.transform.position);
        healthtxt.text = health.ToString();
        float Xaxis = Input.GetAxis("Horizontal") * speed;
        float Zaxis = Input.GetAxis("Vertical") * speed;

        Vector3 movePos = transform.right * Xaxis + transform.forward * Zaxis;
        Vector3 newMovePos = new Vector3(movePos.x, rb.velocity.y, movePos.z);

        rb.velocity = newMovePos;
        if (isDead == true) { gameOverCanvas.SetActive(true); }
        else
        {
            gameOverCanvas.SetActive(false);
            if (distanceBetweenHomeBase <= actionDistance)
            {
                healingCoolDown--;
                if (healingCoolDown <= 10)
                {
                    Heal(maxHealth / 4);
                    healingCoolDown = maxHealCoolDown;
                }
            }

            if (target != null)
            {
                distanceBetweenTarget = Vector3.Distance(this.transform.position, target.transform.position);
                if (distanceBetweenTarget <= actionDistance)
                {
                    canAttack = true;
                }
                if (distanceBetweenTarget > actionDistance)
                {
                    canAttack = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) && canAttack == true)
            {
                enemyScript.TakeDamage(this.attackDamage);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed = speed * 2;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed = originSpeed;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                speed = speed / 3;
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
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag)
        {
            target = other.gameObject;
            enemyScript = target.GetComponent<Enemy>();
        }
    }
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag)
        {
            target = other.gameObject;
            enemyScript = target.GetComponent<Enemy>();
        }
    }

}
