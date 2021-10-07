using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 15;
    public int jumpHeight;
    public bool crouching = false;
    private float originSpeed;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        float Xaxis = Input.GetAxis("Horizontal") * speed;
        float Zaxis = Input.GetAxis("Vertical") * speed;

        Vector3 movePos = transform.right * Xaxis + transform.forward * Zaxis;
        Vector3 newMovePos = new Vector3(movePos.x, rb.velocity.y, movePos.z);

        rb.velocity = newMovePos;

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
            this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x, this.gameObject.transform.localScale.y / 1.5f, this.gameObject.transform.localScale.z);
            crouching = true;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            speed = originSpeed;
            this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x, this.gameObject.transform.localScale.y * 1.5f, this.gameObject.transform.localScale.z);
            crouching = false;
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //transform.Translate(0, jumpHeight * Time.deltaTime, 0);
        //}
    }
}
