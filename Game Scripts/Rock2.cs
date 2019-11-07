using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock2 : MonoBehaviour
{
    public float Min = 100f;
    public float Max = 1500f;

    Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.left * Random.Range(Min, Max), ForceMode2D.Impulse);
    }

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Object" || collision.gameObject.tag == "Player")
            Destroy(rb.gameObject);
    }

}
