using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 15;
    public int jumpHeight;
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
            speed = 30;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = originSpeed;
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
            //transform.Translate(0, jumpHeight * Time.deltaTime, 0);
        //}
    }
}
