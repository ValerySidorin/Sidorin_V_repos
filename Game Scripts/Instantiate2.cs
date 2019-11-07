﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate2 : MonoBehaviour
{
    public GameObject[] objs;
    GameObject gb;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("Parameter", 2);
        Instantiation();
    }

    void Update()
    {
        if (gb == null)
        {
            anim.SetInteger("Parameter", 2);
            Instantiation();
        }
    }

    void Instantiation()
    {
        gb = Instantiate(objs[0], new Vector3(10.5f, 1.35f, 0), Quaternion.identity) as GameObject;
    }
}