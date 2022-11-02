using System;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public GameManager gm;

    public int runSpeed;
    public int jumpForce;
    public int strafeSpeed;

    bool jump = false;
    bool strafeRight = false;
    bool strafeLeft = false;
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag=="obstacle")
        {
            gm.CrashRestart(); 
        }
    }

    void Update()
    {
        if (transform.position.z >= 70)
            gm.NextLoop(ref runSpeed);

        if (transform.position.y<=-1)
            gm.FallRestart(rb);

        if (Input.GetKey(KeyCode.Space) && transform.position.y<=0.5)
            jump = true;
        else jump = false;

        if (Input.GetKey(KeyCode.A))
            strafeLeft = true;
        else strafeLeft = false;

        if (Input.GetKey(KeyCode.D))
            strafeRight = true;
        else strafeRight = false;
    }


    void FixedUpdate()
    {
        //перенести мувмент...? 
        //*баг-влево и вправо с повышением уровн€ замедл€ют игрока при изменении runSpeed

        rb.MovePosition(transform.position + Vector3.forward * runSpeed * Time.deltaTime);

        if (jump)
        {
            //ассет на изменение формы в прыжке
            transform.DORewind();
            transform.DOShakeScale(0.7f,0.5f,4,50);

            rb.AddForce(Vector3.up*jumpForce * Time.deltaTime, ForceMode.Impulse);

        }
            

        if (strafeLeft)
            rb.MovePosition(transform.position + (Vector3.left * strafeSpeed + Vector3.forward * runSpeed)  * Time.deltaTime);

        if (strafeRight)
            rb.MovePosition(transform.position + (Vector3.right * strafeSpeed + Vector3.forward * runSpeed)  * Time.deltaTime);

    }
}
