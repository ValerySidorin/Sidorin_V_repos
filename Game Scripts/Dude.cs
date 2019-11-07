using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dude : MonoBehaviour
{
    public float WalkSpeed = 12f;
    public float JumpForce = 1000f;
    

    Rigidbody2D rb;
    Transform trans;
    Animator anim;
    DirectionState dir = DirectionState.Right;
    bool Ground;
    bool Move = true;
    //bool DoubleJump;
    int Life = 2;
    float time = 60f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    void MoveRight()
    {
        if (dir == DirectionState.Left)
        {
            trans.localScale = new Vector3(-trans.localScale.x, trans.localScale.y, trans.localScale.z);
            dir = DirectionState.Right;
        }
        anim.SetInteger("Parameter", 2);
    }

    void MoveLeft()
    {
        if (dir == DirectionState.Right)
        {
            trans.localScale = new Vector3(-trans.localScale.x, trans.localScale.y, trans.localScale.z);
            dir = DirectionState.Left;
        }
        anim.SetInteger("Parameter", 2);
    }

    void Jump()
    {
        if ((Ground && Move)/* || (DoubleJump && Move)*/)
        {
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            //DoubleJump = false;
        }
    }

    void Update()
    {
        time -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Jump();

        if (Input.GetAxis("Horizontal") == 0)
            anim.SetInteger("Parameter", 3);

        if (Input.GetAxis("Horizontal") < 0)
            MoveLeft();

        if (Input.GetAxis("Horizontal") > 0)
            MoveRight();

        if (!Ground)
            anim.SetInteger("Parameter", 1);

        if (!Move)
            anim.SetInteger("Parameter", 4);


    }

    void Load()
    {
        SceneManager.LoadScene("Menu");
    }

    void FixedUpdate()
    {
        if (Move) rb.velocity = new Vector2(Input.GetAxis("Horizontal") * WalkSpeed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Object")
        {
            Ground = true;
            //DoubleJump = false;
        }

        if (collision.gameObject.tag == "Death")
        {
            Life--;
            if (Life <= 0)
            {
                Move = false;
                Invoke("Death", 1);
            }
        }

        if (collision.gameObject.tag == "Lava")
        {
            Life = 0;
            Move = false;
            Invoke("Death", 1);
        }
            
    }

    void Death()
    {
        Destroy(rb.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Object")
        {
            Ground = false;
            //DoubleJump = true;
        }
        

    }

    void OnGUI()
    {
        GUI.Box(new Rect(385, 50, 100, 20), "Time: " + Math.Round(time, 0));

        GUI.Box(new Rect(0, 0, 100, 20), "Life: " + Life);

        if (time <= 0)
        {
            GUI.Box(new Rect(385, 70, 100, 20), "Victory!");
            Invoke("Load", 1);
        }

        if (!Move)
        {
            GUI.Box(new Rect(385, 70, 100, 20), "You died!");
            Invoke("Load", 1);
        }

    }

    enum DirectionState
    {
        Right,
        Left
    }
}
